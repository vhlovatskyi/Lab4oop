﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Task
{
    // Інтерфейс для типів, які підтримують основні арифметичні операції
    interface IMyNumber<T> where T : IMyNumber<T>
    {
        T Add(T b);       // Додавання
        T Subtract(T b);  // Віднімання
        T Multiply(T b);  // Множення
        T Divide(T b);    // Ділення
    }

    // Клас для роботи з дробами
    class MyFrac : IMyNumber<MyFrac>, IComparable<MyFrac>
    {
        public BigInteger Nom { get; }  // Чисельник дробу
        public BigInteger Denom { get; } // Знаменник дробу

        // Конструктор, який приймає чисельник і знаменник
        public MyFrac(BigInteger nom, BigInteger denom)
        {
            if (denom == 0)
                throw new DivideByZeroException("Denominator cannot be zero."); // Перевірка знаменника на 0

            // Спрощення дробу через знаходження найбільшого спільного дільника
            BigInteger gcd = BigInteger.GreatestCommonDivisor(nom, denom);
            nom /= gcd;
            denom /= gcd;

            // Забезпечення позитивного знаменника
            if (denom < 0)
            {
                nom = -nom;
                denom = -denom;
            }

            Nom = nom;
            Denom = denom;
        }

        // Конструктор для роботи з цілими числами
        public MyFrac(int nom, int denom) : this(new BigInteger(nom), new BigInteger(denom)) { }

        // Додавання дробів
        public MyFrac Add(MyFrac that)
        {
            return new MyFrac(
                Nom * that.Denom + that.Nom * Denom, // Приведення до спільного знаменника
                Denom * that.Denom
            );
        }

        // Віднімання дробів
        public MyFrac Subtract(MyFrac that)
        {
            return new MyFrac(
                Nom * that.Denom - that.Nom * Denom,
                Denom * that.Denom
            );
        }

        // Множення дробів
        public MyFrac Multiply(MyFrac that)
        {
            return new MyFrac(
                Nom * that.Nom,
                Denom * that.Denom
            );
        }

        // Ділення дробів
        public MyFrac Divide(MyFrac that)
        {
            if (that.Nom == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            return new MyFrac(
                Nom * that.Denom,
                Denom * that.Nom
            );
        }

        // Перетворення дробу у рядок
        public override string ToString() => $"{Nom}/{Denom}";

        // Порівняння дробів для сортування
        public int CompareTo(MyFrac that)
        {
            BigInteger diff = Nom * that.Denom - that.Nom * Denom; // Вирахування різниці
            return diff.Sign; // Знак різниці визначає порядок
        }

        // Оператори для арифметичних операцій
        public static MyFrac operator +(MyFrac a, MyFrac b) => a.Add(b);
        public static MyFrac operator -(MyFrac a, MyFrac b) => a.Subtract(b);
        public static MyFrac operator *(MyFrac a, MyFrac b) => a.Multiply(b);
        public static MyFrac operator /(MyFrac a, MyFrac b) => a.Divide(b);
    }

    // Клас для роботи з комплексними числами
    class MyComplex : IMyNumber<MyComplex>
    {
        public double Re { get; } // Дійсна частина
        public double Im { get; } // Уявна частина

        // Конструктор
        public MyComplex(double re, double im)
        {
            Re = re;
            Im = im;
        }

        // Додавання комплексних чисел
        public MyComplex Add(MyComplex that)
        {
            return new MyComplex(Re + that.Re, Im + that.Im);
        }

        // Віднімання комплексних чисел
        public MyComplex Subtract(MyComplex that)
        {
            return new MyComplex(Re - that.Re, Im - that.Im);
        }

        // Множення комплексних чисел
        public MyComplex Multiply(MyComplex that)
        {
            return new MyComplex(
                Re * that.Re - Im * that.Im, // Реальна частина
                Re * that.Im + Im * that.Re  // Уявна частина
            );
        }

        // Ділення комплексних чисел
        public MyComplex Divide(MyComplex that)
        {
            double divisor = that.Re * that.Re + that.Im * that.Im;
            if (divisor == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            return new MyComplex(
                (Re * that.Re + Im * that.Im) / divisor,
                (Im * that.Re - Re * that.Im) / divisor
            );
        }

        // Перетворення комплексного числа у рядок
        public override string ToString() => $"{Re}+{Im}i";

        // Оператори для арифметичних операцій
        public static MyComplex operator +(MyComplex a, MyComplex b) => a.Add(b);
        public static MyComplex operator -(MyComplex a, MyComplex b) => a.Subtract(b);
        public static MyComplex operator *(MyComplex a, MyComplex b) => a.Multiply(b);
        public static MyComplex operator /(MyComplex a, MyComplex b) => a.Divide(b);
    }

    class Program
    {
        // Тест рівності (a-b)^2 = a^2 - 2ab + b^2
        static void TestSquaresDifference<T>(T a, T b) where T : IMyNumber<T>
        {
            Console.WriteLine("\nTestSquaresDifference:");
            Console.WriteLine($"=== Testing (a-b)^2 = a^2 - 2ab + b^2 with a = {a}, b = {b} ===");

            T aMinusB = a.Subtract(b);
            T a2 = a.Multiply(a);
            T b2 = b.Multiply(b);
            T twoAB = a.Multiply(b).Add(a.Multiply(b)); // 2ab = ab + ab

            Console.WriteLine($"(a-b) = {aMinusB}");
            Console.WriteLine($"(a-b)^2 = {aMinusB.Multiply(aMinusB)}");
            Console.WriteLine($"a^2 - 2ab + b^2 = {a2.Subtract(twoAB).Add(b2)}");
        }

        // Тест рівності (a+b)^2 = a^2 + 2ab + b^2
        static void TestAPlusBSquare<T>(T a, T b) where T : IMyNumber<T>
        {
            Console.WriteLine("\nTestAPlusBSquare:");
            Console.WriteLine($"=== Testing (a+b)^2 = a^2 + 2ab + b^2 with a = {a}, b = {b} ===");

            T aPlusB = a.Add(b);
            T a2 = a.Multiply(a);
            T b2 = b.Multiply(b);
            T twoAB = a.Multiply(b).Add(a.Multiply(b));

            Console.WriteLine($"(a+b)^2 = {aPlusB.Multiply(aPlusB)}");
            Console.WriteLine($"a^2 + 2ab + b^2 = {a2.Add(twoAB).Add(b2)}");
        }

        static void Main(string[] args)
        {
            var fractions = new List<MyFrac>
            {
                new MyFrac(1, 3),
                new MyFrac(2, 3),
                new MyFrac(1, 6)
            };

            Console.WriteLine("Before sorting:");
            foreach (var frac in fractions)
                Console.WriteLine(frac);

            // Сортування дробів
            fractions.Sort();

            Console.WriteLine("\nAfter sorting:");
            foreach (var frac in fractions)
                Console.WriteLine(frac);

            // Тестування рівностей
            TestAPlusBSquare(new MyFrac(1, 3), new MyFrac(1, 6));
            TestSquaresDifference(new MyFrac(1, 3), new MyFrac(1, 6));

            TestAPlusBSquare(new MyComplex(1, 3), new MyComplex(1, 6));
            TestSquaresDifference(new MyComplex(1, 3), new MyComplex(1, 6));

            Console.ReadKey();
        }
    }
}