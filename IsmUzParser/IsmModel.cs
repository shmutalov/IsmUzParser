using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsmUzParser
{
    /// <summary>
    /// Пол
    /// </summary>
    public enum GENDER
    {
        MALE
        , FEMALE
    }

    /// <summary>
    /// Информация об имени
    /// </summary>
    public class IsmModel
    {
        /// <summary>
        /// Начальная буква имени
        /// </summary>
        public string Letter { get; set; }
        /// <summary>
        /// Пол
        /// </summary>
        public GENDER Gender { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Происхождение
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public string Meaning { get; set; }

        public IsmModel(string letter, GENDER gender, string name, string origin, string meaning)
        {
            Letter = letter;
            Gender = gender;
            Name = name;
            Origin = origin;
            Meaning = meaning;
        }
    }
}
