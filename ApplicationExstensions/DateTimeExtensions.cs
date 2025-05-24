namespace DatingApp_API.ApplicationExstensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            var age = now.Year - dob.Year;

            if (dob > now.AddYears(-age)) 
            {
                age--;
            }

            return age;
        }
    }
}
