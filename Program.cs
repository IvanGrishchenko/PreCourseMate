using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using preCourse.Contest;
namespace preCourse
{
    namespace Math
    {
        public class Randomness
        {
            #region Fields
            private float maxValue;
            private int precision;
            private Random random;
            #endregion
            #region Constructors
            public Randomness(float maxValue, int precision)
            {
                this.maxValue = maxValue;
                this.precision = precision;
                random = new Random();
            }
            #endregion
            #region Methods
            public float Next()
            {
                float next = (float)random.NextDouble() * maxValue;
                next = CommonMethods.GetWithPrecision(next, precision);
                return next;
            }

            #endregion
        }
        public static class CommonMethods
        {
            public static float GetWithPrecision(float value, int precision)
            {
                float withPrecision;
                int multiplier = (int)System.Math.Pow(10, precision);
                withPrecision = ((int)(value * multiplier)) / (float)multiplier;
                return withPrecision;
            }
        }
    }
    namespace Contest
    {
        public class Performer
        {
            #region Constructors
            static Performer()
            {
                randomness = new Math.Randomness(MAX_MARK, 1);
            }
            public Performer(string name = "UnKnown")
            {
                this.Name = name;
            }
            #endregion
            #region Fields
            public const float MAX_MARK = 6f;

            private static Math.Randomness randomness;
            private float averageJuryMark;
            #endregion
            #region Properties
            public string Name { get; private set; }
            public float PerformMark { get; private set; }
            public float AverageJuryMark
            {
                set
                {
                    if (averageJuryMark < 0f || averageJuryMark > 6f)
                        throw new Exception("Unacceptable mark");
                    averageJuryMark = value;
                }
                get => averageJuryMark;
            }
            #endregion
            #region Methods
            public Performer Perform()
            {
                PerformMark = randomness.Next();
                return this;
            }

            #endregion
        }
        public class Jury
        {
            private static Random random = new System.Random();
            private string name;
            public Jury(string name = "just ordinary jury")
            {
                this.name = name;
            }
            private float RateViaMark(float mark)
            {
                bool increase = random.NextDouble() > 0.5;
                float juryMark;
                float maxMark = Performer.MAX_MARK;
                float delta;
                if (increase)
                {
                    delta = maxMark - mark;
                }
                else
                {
                    delta = (-1f) * mark;
                }
                juryMark = mark + (float)random.NextDouble() * delta;
                juryMark = Math.CommonMethods.GetWithPrecision(juryMark, 1);
                return juryMark;
            }
            public float RatePerformer(Performer performer)
            {
                return RateViaMark(performer.PerformMark);
            }
        }
        public class JuryCommittee
        {
            private Jury[] committee;
            public JuryCommittee(Jury[] committee)
            {
                this.committee = committee;
            }
            public Performer RatePerformer(Performer performer)
            {
                List<float> marks = committee.
                    Select(jury => jury.RatePerformer(performer)).
                    OrderBy(x => (int)x).
                    ToList();
                marks.RemoveAt(0);
                marks.RemoveAt(marks.Count - 1);
                float sum = marks.Sum();

                float averageMark = sum / marks.Count;

                averageMark = Math.CommonMethods.GetWithPrecision(averageMark, 1);
                performer.AverageJuryMark = averageMark;
                return performer;
            }
            public void AnnounceWinners(List<Performer> perfomers)
            {
                Performer[] sortedPerformers = perfomers.OrderByDescending(x => x.AverageJuryMark).ToArray();
                Console.WriteLine("Winners:");
                for (int place = 1; place <= 3; place++)
                {
                    Console.WriteLine($"{place}: {sortedPerformers[place - 1].Name}");
                }
            }

            public void AnnounceResults(List<Performer> perfomers)
            {
                Console.WriteLine("Results:");
                foreach (var perfomer in perfomers)
                {
                    Console.WriteLine($"\t{perfomer.Name}:\t{perfomer.AverageJuryMark}");
                }
            }
        }
        public class Competition
        {
            private JuryCommittee committee;
            private List<Performer> perfomers;
            public Competition(JuryCommittee committee, List<Performer> perfomers)
            {
                this.committee = committee;
                this.perfomers = perfomers;
            }
            public void ConductCompetition()
            {
                foreach (var perfomer in perfomers)
                {
                    perfomer.Perform();
                    committee.RatePerformer(perfomer);
                }

                committee.AnnounceWinners(perfomers);
                committee.AnnounceResults(perfomers);

            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfPerfomers;
            Console.WriteLine("Enter number of perfomers:");
            string input = Console.ReadLine();
            bool validInput = Int32.TryParse(input, out numberOfPerfomers);
            if (!validInput)
                throw new Exception("Invalid input");
            Console.WriteLine("Enter perfomer's names one by one:");
            List<Performer> perfomers = new List<Performer>();
            for (int i = 0; i < numberOfPerfomers; i++)
            {
                string name = Console.ReadLine();
                perfomers.Add(new Performer(name));
            }
            var juries = new List<Jury>();
            for (int i = 0; i < 17; i++)
            {
                juries.Add(new Jury());
            }
            JuryCommittee committee = new JuryCommittee(juries.ToArray());
            new Competition(committee, perfomers).ConductCompetition();
            Console.ReadKey();
        }
    }
}
