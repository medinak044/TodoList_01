namespace TodoList_01_API.Helpers;

public static class StringExtensions
{
    // Input string must have "Controller" for this method to work
    public static string GetControllerName(this string str)
    {
        int position = str.IndexOf("Controller");

        return str.Substring(0, position); // Returns new string without "Controller"
    }
}
