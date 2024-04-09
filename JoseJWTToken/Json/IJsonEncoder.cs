namespace JoseJWTToken.Json
{
    public interface IJsonEncoder
    {
        TResult DecodeObject<TResult>(string data);
        string EncodeObject(object obj);
    }
}