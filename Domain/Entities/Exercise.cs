namespace Domain.Entities
{
    public class Exercise
    {
        public int Id { get; set; }
        public string ExerciseName { get; set; }
        public int Series { get; set; }
        public int Repetitions { get; set; }
    }
}
