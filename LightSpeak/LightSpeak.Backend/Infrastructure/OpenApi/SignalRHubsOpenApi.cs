namespace LightSpeak.Backend.Infrastructure.OpenApi;

using System.Net.Http;
using Microsoft.OpenApi;

public static class SignalRHubsOpenApi
{
    public static void AddSignalRHubsDocumentation(this OpenApiDocument document)
    {
        document.Tags ??= new HashSet<OpenApiTag>();
        document.Tags.Add(new OpenApiTag
        {
            Name = "ChatHub",
            Description =
                "SignalR hub for real-time text chat (WebSocket). " +
                "Documentation only — not a REST endpoint.",
        });
        document.Tags.Add(new OpenApiTag
        {
            Name = "VoiceHub",
            Description =
                "SignalR hub for WebRTC voice signaling (WebSocket). " +
                "Documentation only — not a REST endpoint. Media travels P2P.",
        });

        document.Paths["/hubs/chat"] = CreateHubPath(
            document,
            tag: "ChatHub",
            summary: "ChatHub — real-time channel messages",
            description: ChatHubDescription);

        document.Paths["/hubs/voice"] = CreateHubPath(
            document,
            tag: "VoiceHub",
            summary: "VoiceHub — WebRTC signaling only",
            description: VoiceHubDescription);
    }

    private static OpenApiPathItem CreateHubPath(
        OpenApiDocument document,
        string tag,
        string summary,
        string description)
    {
        return new OpenApiPathItem
        {
            Description = summary,
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Tags = new HashSet<OpenApiTagReference> { new(tag, document) },
                    Summary = summary,
                    Description = description,
                    Security = new List<OpenApiSecurityRequirement>
                    {
                        new()
                        {
                            [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
                        },
                    },
                    Responses = new OpenApiResponses
                    {
                        ["101"] = new OpenApiResponse
                        {
                            Description =
                                "Switching Protocols — WebSocket upgrade. " +
                                "Invoke hub methods after connect; this is not a normal HTTP GET.",
                        },
                    },
                },
            },
        };
    }

    private const string ChatHubDescription =
        """
        > **SignalR hub (WebSocket)** — docs only, not a REST API.
        >
        > **URL:** `/hubs/chat`
        >
        > **Auth:** JWT as `?access_token=<token>` or cookie `accessToken`.
        >
        > Front: `npm i @microsoft/signalr`

        ### Client → server (`connection.invoke`)

        | Method | Parameters |
        | ------ | ---------- |
        | `JoinChannel` | `channelId: Guid` |
        | `LeaveChannel` | `channelId: Guid` |
        | `SendMessage` | `channelId: Guid`, `content: string` |
        | `Typing` | `channelId: Guid` |

        - **JoinChannel** — adds the connection to group `channel-{id}`. Fails with `HubException` if not a server member.
        - **LeaveChannel** — removes the connection from the group.
        - **SendMessage** — persists the message (CQRS) and broadcasts `ReceiveMessage` to the group. Rate-limited; failures throw `HubException`.
        - **Typing** — notifies other members with `UserTyping` (not sent to the caller).

        ### Server → client (`connection.on`)

        | Event | Payload |
        | ----- | ------- |
        | `ReceiveMessage` | `{ id, channelId, authorId, authorFirstName, authorLastName, authorProfileImageUrl, content, createdAt, editedAt }` |
        | `UserTyping` | `userId: Guid`, `username: string` |

        ### Typical flow

        1. `connection.on("ReceiveMessage" | "UserTyping", ...)`
        2. `await connection.start()`
        3. `await connection.invoke("JoinChannel", channelId)`
        4. `await connection.invoke("SendMessage", channelId, "hello")`
        5. On channel switch: `LeaveChannel` → `JoinChannel`

        Message history is **REST**: `GET /api/v1/channels/{channelId}/messages`.
        """;

    private const string VoiceHubDescription =
        """
        > **SignalR hub (WebSocket)** — docs only, not a REST API.
        >
        > **URL:** `/hubs/voice`
        >
        > **Auth:** JWT as `?access_token=<token>` or cookie `accessToken`.
        >
        > Audio/video does **not** go through this hub — only WebRTC signaling (SDP/ICE). Media is P2P.

        ### Client → server (`connection.invoke`)

        | Method | Parameters |
        | ------ | ---------- |
        | `JoinVoiceChannel` | `channelId: Guid` |
        | `LeaveVoiceChannel` | `channelId: Guid` |
        | `SendSignal` | `targetConnectionId: string`, `signalType: "offer" \| "answer" \| "ice-candidate"`, `payload: string` |

        - **JoinVoiceChannel** — register in the room (call after `getUserMedia`). Server replies with `ExistingPeers` and notifies others with `PeerJoined`.
        - **LeaveVoiceChannel** — leave room; others get `PeerLeft`.
        - **SendSignal** — relay one signaling message to a peer in the **same** room. `payload` is `JSON.stringify(...)` of the SDP or ICE candidate.

        ### Server → client (`connection.on`)

        | Event | Payload |
        | ----- | ------- |
        | `ExistingPeers` | `[{ connectionId, userId, username }]` — peers already in the room when you join |
        | `PeerJoined` | `{ connectionId, userId, username }` |
        | `PeerLeft` | `connectionId: string` |
        | `ReceiveSignal` | `{ fromConnectionId, fromUserId, signalType, payload }` |

        ### WebRTC flow

        1. Join → `ExistingPeers` → **new client creates offers** to each peer (`SendSignal(..., "offer", ...)`).
        2. Remote receives `ReceiveSignal` `offer` → `createAnswer` → `SendSignal(..., "answer", ...)`.
        3. Both sides exchange `ice-candidate` the same way.
        4. `ontrack` → play remote audio (`<audio autoplay>`).
        5. Leave → `LeaveVoiceChannel` + close all `RTCPeerConnection`s.

        Payloads from the server use **camelCase** property names.
        """;
}
