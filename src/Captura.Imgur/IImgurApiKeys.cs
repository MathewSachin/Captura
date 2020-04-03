namespace Captura.Imgur
{
    public interface IImgurApiKeys
    {
        string ImgurClientId { get; }

        string ImgurSecret { get; }
    }
}