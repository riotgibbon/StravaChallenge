using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IdentityManagement.Dto;
using IdentityManagement.Models;

namespace IdentityManagement
{
    public class Mapping
    {
        public static Identity MapOpenAMIdentity(OpenAMIdentity openAMidentity)
        {
            return new Identity
            {
                Email = GetFieldValueFromIdentity(openAMidentity, "mail"),
                FirstName = GetFieldValueFromIdentity(openAMidentity, "givenname"),
                LastName = GetFieldValueFromIdentity(openAMidentity, "sn"),
                Roles = GetSeperatedList(openAMidentity, ',')
            };
        }

        private static List<string> GetSeperatedList(OpenAMIdentity openAMidentity, char separator)
        {
            var listAttribute = openAMidentity.roles.FirstOrDefault();
            return listAttribute != null ? listAttribute.Split(separator).ToList() : null;
        }

        private static string GetFieldValueFromIdentity(OpenAMIdentity openAMidentity, string fieldName)
        {
            string result = null;
            var attribute = openAMidentity.attributes.FirstOrDefault(a => a.name == fieldName);

            if (attribute != null)
                result = attribute.values.FirstOrDefault();
            return result;
        }

        public static Identity MapPropertyList(string propertyList)
        {
            var properties = Regex.Split(propertyList, "\r\n|\r|\n");
            var names = GetAttributes(properties, "userdetails.attribute.name");
            var values = GetAttributes(properties, "userdetails.attribute.value");
            var propertyDictionary = new Dictionary<string, string>();
            for (int i = 0; i < names.Length; i++)
            {
                propertyDictionary.Add(GetAttributeValue(names[i]), GetAttributeValue(values[i]));
            }
            var identity = new Identity
            {
                FirstName = propertyDictionary["givenname"],
                LastName = propertyDictionary["sn"],
                Email = propertyDictionary["mail"]
            };
            return identity;
        }
        private static string[] GetAttributes(string[] properties, string attributeName)
        {
            return properties.Where(s => s.StartsWith(attributeName)).ToArray();
        }

        private static string GetAttributeValue(string attribute)
        {
            var attributeValue = attribute.Split('=')[1];
            return attributeValue;
        }


    }
}