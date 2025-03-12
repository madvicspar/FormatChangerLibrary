using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class DocumentModel
    {
        public long Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public string FilePath { get; set; }
        /// <summary>
        /// Оригинальная версия документа или исправленная/проверенная/список недочетов
        /// </summary>
        public bool IsOriginal { get; set; }
        public UserModel User { get; set; }
    }
}
