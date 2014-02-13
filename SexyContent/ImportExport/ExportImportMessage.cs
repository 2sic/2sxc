namespace ToSic.SexyContent.ImportExport
{
    /// <summary>
    /// Describes a Message while Exporting / Importing
    /// </summary>
    public class ExportImportMessage
    {
        public ExportImportMessage(string Message, MessageTypes MessageType)
        {
            this.Message = Message;
            this.MessageType = MessageType;
        }

        public string Message { get; set; }
        public MessageTypes MessageType { get; set; }
        public enum MessageTypes { Warning, Information, Error }
    }
}