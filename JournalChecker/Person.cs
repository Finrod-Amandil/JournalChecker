/*
 * PERSON.CS
 * JournalChecker module
 * 
 * Module description:
 * -------------------
 * Person.cs contains the following functionalities:
 * - Auxiliary class Person POJO
 * 
 */

namespace JournalChecker
{
    class Person
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName { get; private set; }

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            FullName = lastName + " " + firstName;
        }
    }
}
