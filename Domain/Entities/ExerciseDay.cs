using Domain.Enums;

namespace Domain.Entities
{
    public class ExerciseDay
    {
        public int Id { get; set; }
        public WeekDay WeekDay { get; set; }
        public List<Exercise>? Exercises { get; set; }
    }
}
