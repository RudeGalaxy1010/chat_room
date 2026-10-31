namespace App.Domains.Network.Api
{
    public interface IMessageEncoder
    {
        string Encode<T>(T message);
        T Decode<T>(string message);
    }
}