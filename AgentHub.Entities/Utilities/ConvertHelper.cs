namespace AgentHub.Entities.Utilities
{
    public static class ConvertHelper
    {
        public static string ToString(this object value)
        {
            return value != null ? value.ToString() : string.Empty;
        }

        public static int ToInt(this string value)
        {
            int result;
            int.TryParse(value, out result);

            return result;
        }

        public static int ToInt(this object value)
        {
            var valueInString = (value != null ? value.ToString() : "");

            return valueInString.ToInt();
        }

        public static double ToDouble(this string value)
        {
            double result;
            double.TryParse(value, out result);

            return result;
        }

        public static bool ToBool(this string value)
        {
            bool result;
            if (!bool.TryParse(value, out result))
                result = false;

            return result;
        }
    }
}
