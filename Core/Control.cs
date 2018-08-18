using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;

namespace Core
{
    public class Control
    {
        public class ValueInvaException : Exception
        {
            public ValueInvaException(string message) : base(message)
            {
            }
        }

        Core Core;

        TextBox StudentID_TextBox;
        public long StudentID
        {
            get
            {
                return long.Parse(StudentID_TextBox.Text);
            }
            set
            {
                if (value != 0)
                    StudentID_TextBox.Text = value.ToString();
                else
                    StudentID_TextBox.Text = "";
            }
        }
        public int CardID { get; set; }
        public double Balance { get; set; }

        public int Apple { get; set; }
        public int CardTypeID { get; set; }
        public char Sex { get; set; }
        public int Age { get; set; }

        public string StudentName { get; set; }
        public string PhoneNumber { get; set; }
        public string School { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }

        public DateTime Birthday { get; set; }

        public double TotalMoney { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LastSignIn { get; set; }

        void FindStudent()
        {
            if (string.IsNullOrEmpty(StudentID_TextBox.Text))
            {
                //TODO
            }
            else
            {
                long studentID = 0;
                try
                {
                    studentID = StudentID;
                    if (studentID > Core.Settings.CardIDMax)
                    {
                        if (Core.FindStudent(StudentID))
                        {
                            //TODO
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (Core.FindStudent((int)StudentID))
                        {
                            //TODO
                        }
                        else
                        {

                        }
                    }
                }
                catch
                {
                    if (Core.FindStudent(StudentID_TextBox.Text))
                    {
                        //TODO
                    }
                    else
                    {

                    }
                }

            }
        }
        void SignIn()
        {
            if (Core.Student != null)
            {
                Core.SignIn();
                //TODO
            }
            else
            {

            }
        }


        void Print()
        {

        }
    }
}