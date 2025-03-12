namespace FormatChanger.Models
{
    public enum Ends
    {
        /// <summary>
        /// На конце элемента списка ничего нет
        /// Символ: ''
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// На конце элемента списка точка
        /// Символ: '.'
        /// </summary>
        Period = 1,
        /// <summary>
        /// На конце элемента списка точка с запятой
        /// Символ: ';'
        /// </summary>
        Semicolon = 2,
        /// <summary>
        /// На конце элемента списка запятая
        /// Символ: ','
        /// </summary>
        Comma = 3
    }
}
