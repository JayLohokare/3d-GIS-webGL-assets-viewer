using System;
using System.ComponentModel.DataAnnotations;

namespace ModelService
{
    public class Model
    {

        [Key]
        public string ModelName { get; set; }
        public float X_Scale { get; set; }
        public float Y_Scale { get; set; }
        public float Z_Scale { get; set; }
        public float Z_Position { get; set; }
    }
}