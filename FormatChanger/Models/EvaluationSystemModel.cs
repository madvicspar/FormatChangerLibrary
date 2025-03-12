namespace FormatChanger.Models
{
    public class EvaluationSystemModel
    {
        public long Id { get; set; }
        /// <summary>
        /// Вес заголовков от о до 100
        /// </summary>
        public int HeaderWeight { get; set; }

        /// <summary>
        /// Вес обычного текста от о до 100
        /// </summary>
        public int TextWeight { get; set; }

        /// <summary>
        /// Вес изображений от о до 100
        /// </summary>
        public int ImageWeight { get; set; }

        /// <summary>
        /// Вес таблиц от о до 100
        /// </summary>
        public int TableWeight { get; set; }

        /// <summary>
        /// Вес списков от о до 100
        /// </summary>
        public int ListWeight { get; set; }

        ///// <summary>
        ///// Вес кода от о до 100
        ///// </summary>
        //public int CodeWeight { get; set; }

        /// <summary>
        /// Свободный коэффициент от о до 100
        /// </summary>
        public int FreeCoefficient { get; set; }
        /// <summary>
        /// Метод для проверки, что сумма всех коэффициентов равна 100.
        /// </summary>
        public bool CheckTotalWeight()
        {
            int totalWeight = HeaderWeight + TextWeight + ImageWeight + TableWeight + ListWeight + FreeCoefficient;
            return totalWeight == 100;
        }
    }
}
