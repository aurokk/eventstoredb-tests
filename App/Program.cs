using System.Text;
using EventStore.Client;

const string connectionString = "esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";
var settings = EventStoreClientSettings.Create(connectionString);
await using var client = new EventStoreClient(settings);

var eventData = new EventData(
    Uuid.NewUuid(),
    "some-event",
    Encoding.UTF8.GetBytes("{\"id\": \"1\" \"value\": \"some value\"}")
);

await client.AppendToStreamAsync(
    "some-stream",
    StreamState.NoStream,
    new List<EventData>
    {
        eventData
    });

var result = client.ReadStreamAsync(Direction.Forwards, "some-stream", StreamPosition.Start);
var messages = await result.ToArrayAsync();

foreach (var message in messages)
{
    Console.WriteLine(Encoding.UTF8.GetString(message.Event.Data.Span));
}