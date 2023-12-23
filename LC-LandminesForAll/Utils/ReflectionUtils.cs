using System;

namespace LC_LandminesForAll.Utils
{
    internal static class ReflectionUtils
    {
        public static void SetPrivateField<T>(this T instance, string fieldName, object value)
        {
            typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(instance, value);
        }

        public static void SetPrivateProperty<T>(this T instance, string propertyName, object value)
        {
            typeof(T).GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(instance, value);
        }

        public static void SetPrivateStaticField<T>(string fieldName, object value)
        {
            typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, value);
        }

        public static void SetPrivateStaticProperty<T>(string propertyName, object value)
        {
            typeof(T).GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, value);
        }

        public static T GetPrivateField<T>(this object instance, string fieldName)
        {
            return (T)instance.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(instance);
        }

        public static T GetPrivateProperty<T>(this object instance, string propertyName)
        {
            return (T)instance.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(instance);
        }

        public static T GetPrivateStaticField<T>(string fieldName)
        {
            return (T)typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
        }

        public static T GetPrivateStaticProperty<T>(string propertyName)
        {
            return (T)typeof(T).GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
        }

        public static void InvokePrivateMethod<T>(this T instance, string methodName, params object[] parameters)
        {
            instance.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(instance, parameters);
        }

        public static void InvokePrivateStaticMethod<T>(string methodName, params object[] parameters)
        {
            typeof(T).GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, parameters);
        }

        public static T CreatePrivateInstance<T>(params object[] parameters)
        {
            return (T)Activator.CreateInstance(typeof(T), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, parameters, null);
        }

        public static T CreatePrivateStaticInstance<T>(params object[] parameters)
        {
            return (T)Activator.CreateInstance(typeof(T), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, null, parameters, null);
        }
    }
}
