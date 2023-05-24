﻿namespace KeycloakProvider;

public static class UsersExtenders
{
    public static T Enabled<T>(this T o, bool enabled = true) where T : KeycloakModifyUser
    {
        o.Values["enabled"] = enabled;
        return o;
    }

    public static T UserName<T>(this T o, string userName) where T : KeycloakModifyUser
    {
        ArgumentNullException.ThrowIfNull(userName);
        o.Values["username"] = userName;

        return o;
    }

    public static T Name<T>(this T o, string firstName, string? lastName = null) where T : KeycloakModifyUser
    {
        ArgumentNullException.ThrowIfNull(firstName);
        o.Values["firstName"] = firstName;

        if (lastName != null)
            o.Values["lastName"] = lastName;
        return o;
    }

    public static T Attributes<T>(this T o, Dictionary<string, string> attributes) where T : KeycloakModifyUser
    {
        ArgumentNullException.ThrowIfNull(attributes);
        o.Values["attributes"] = attributes;
        return o;
    }

    public static T AddAttribute<T>(this T o, string name, string value) where T : KeycloakModifyUser
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        if (!o.Values.TryGetValue("attributes", out var attr))
            o.Values.Add("attributes", attr = new Dictionary<string, string>());

        ((Dictionary<string, string>) attr)[name] = value;
        return o;
    }

    public static T Password<T>(this T o, string password, bool temporary = false) where T : KeycloakModifyUser
    {
        ArgumentNullException.ThrowIfNull(password);
        o.Values["credentials"] = new[]
                                  {
                                      new
                                      {
                                          temporary = temporary,
                                          value     = password,
                                          type      = "password"
                                      }
                                  };
        return o;
    }

    public static Dictionary<string, string> MergeAttributes(this Dictionary<string, string>? existingAttributes, Dictionary<string, string?> attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        existingAttributes ??= new Dictionary<string, string>();
        foreach (var attr in attributes)
        {
            if (attr.Value == null) existingAttributes.Remove(attr.Key);
            else existingAttributes[attr.Key] = attr.Value;
        }

        return existingAttributes;
    }
}