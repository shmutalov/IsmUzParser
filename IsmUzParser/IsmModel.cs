using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsmUzParser
{
    public enum GENDER
    {
        MALE
        , FEMALE
    }

    class IsmModel
    {
        public string Letter { get; set; }
        public GENDER Gender { get; set; }
        public string Name { get; set; }
        public string Meaning { get; set; }

        public IsmModel(string letter, GENDER gender, string name, string meaning)
        {
            Letter = letter;
            Gender = gender;
            Name = name;
            Meaning = meaning;
        }
    }
}
