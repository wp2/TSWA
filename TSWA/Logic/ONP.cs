using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _ONP
{
    enum Associativity :int
    {
        LEFT,
        RIGHT
    }
    
    abstract class Token // Klasa bazowa dla tokenu zawiera nazwę która jest wspólna dla wszystkich innych klas pochodnych
    {
        public readonly string Name;
        public Token(string Name)
        {
            this.Name = Name;
        }
    }
    
    class Number : Token // Każda liczba 
    {
        public Decimal Value;
        public Number(string Name):base(Name) { } // Kiedy obliczamy wynik wyrażeń czy to operatorów czy całej notcji ONP
        //możemy użyć tej klasy do reprezentacji wyniku
        public Number(string Name, Decimal value) : base(Name)
        {
            this.Value = value;
        }

        public virtual Decimal ReturnDecimalValue() { return Value; } // Każdy system liczbowy musi zwócić liczbę w systeie dziesiętnym     

      
        // jednakże implementacja tej metody zależy od konkretnego oiektu

    }
    abstract class Functions : Token // Funkcje Sin,Cos,SQRT .....
    {
        protected Decimal[] FunctionParams; // Parametry Funkcji (a,b .......)
        public Functions(string Name) : base(Name)
        {

        }
        public abstract Number FunctionsResult();
        
    }    
    abstract class Operator : Token  // Operatory + , - ,* , /
    {
        int Priority;
        public readonly char Symbol;
        public int GetPriority()
        {
            return Priority;
        }
        int Associativity;
        public int GetAssociativity()
        {
            return Associativity;
        }
        public Operator(string Name, int Priority, char Symbol, int Associativity) : base(Name)
        {
            this.Priority = Priority;
            this.Symbol = Symbol;
            this.Associativity = Associativity;
        }
        //abstract public Number<V> OperatorResult<V>(Number<V> p1, Number<V> p2);
        
    }
    abstract class ArithmeticOperators : Operator
    {
        public ArithmeticOperators(string Name, int Priority, char Symbol, int Associativity) : base(Name, Priority, Symbol, Associativity)
        {
            
        }
        abstract public Number OperatorResult(Number p1, Number p2);
    }
    abstract class SpecialOperator : Operator
    {
        // Klasa ta jest przeznaczona dla operatrów specjalnych czyli takich które pełnią jakąś rolę podczas parsowania
        // wyrażeń ale nie wykonują działań arytmetycznych dobrym przykładem tutaj są nawiasy lewy jak i prawy '('  ')' 
        public SpecialOperator(string Name, int Priority, char Symbol, int Associativity) : base(Name, Priority, Symbol, Associativity)
        {
        }
    }


    /********************* DEFINICJA TOKENÓW *****************************/
    class NawiasLewy : SpecialOperator
    {

        public NawiasLewy(string Name = "Nawias Lewy", int Priority = 0,
            char Symbol = '(', int Associativity = (int)Associativity.LEFT) : base(Name, Priority, Symbol,Associativity)
        {
            // Własny Setup Operatora
            

        }
       
    }
    class NawiasPrawy : SpecialOperator
    {
        public NawiasPrawy(string Name = "NawiasPrawy", int Priority = 0,
            char Symbol = ')', int Associativity = (int)Associativity.LEFT) : base(Name, Priority, Symbol,Associativity)
        {
        }
    }
    class Dodawanie : ArithmeticOperators
    {
        public Dodawanie(string Name = "Dodawanie", int Priority = 1,
            char Symbol = '+', int Associativity = (int)Associativity.LEFT) : base(Name, Priority, Symbol, Associativity)
        {

        }
        public override Number OperatorResult(Number p1, Number p2)
        {
            return new Number("WynikOperacji", (Decimal)p1.Value + p2.Value);
        }        
    }
    class Odejmowanie : ArithmeticOperators
    {
        public Odejmowanie(string Name = "Odejmowanie", int Priority = 1,
            char Symbol = '-', int Associativity = (int)Associativity.LEFT) : base(Name, Priority, Symbol, Associativity)
        {
        }
        public override Number OperatorResult(Number p1, Number p2)
        {
            return new Number("WynikOperacji", (Decimal)p1.Value - p2.Value);
        }

    }
    class Mnożenie : ArithmeticOperators
    {
        public Mnożenie(string Name = "Mnożenie", int Priority = 2,
            char Symbol = '*', int Associativity = (int)Associativity.LEFT) : base(Name, Priority, Symbol, Associativity)
        {
        }
        public override Number OperatorResult(Number p1, Number p2)
        {
            return new Number("WynikOperacji", (Decimal)p1.Value * p2.Value);
        }        
    }
    class Dzielenie : ArithmeticOperators
    {
        public Dzielenie(string Name = "Dzielenie", int Priority = 2,
            char Symbol = '/', int Associativity = (int)Associativity.LEFT) : base(Name, Priority, Symbol, Associativity)
        {
        }
        public override Number OperatorResult(Number p1, Number p2)
        {
            return new Number("WynikOperacji", (Decimal)p1.Value / p2.Value);
        }       
    }
    class Potęga : ArithmeticOperators
    {
        public Potęga(string Name = "Potęga", int Priority = 3,
            char Symbol = '^', int Associativity = (int)Associativity.RIGHT) : base(Name, Priority, Symbol, Associativity)
        {
        }

        public override Number OperatorResult(Number p1, Number p2)
        {
            double x = (double)p1.Value;
            double a = (double)p2.Value;
            double result = Math.Pow(x, a);
            return new Number("WynikOperacji", (Decimal)result );
        }
    }
    class Cyfra : Number
    {
        public Cyfra(string Name, Decimal value) : base(Name, value)
        {
            this.Value = value;
        }

        public override Decimal ReturnDecimalValue()
        {
            return Value;
        }
        public int ReturnBinaryValue()
        {
            // TO DO
            return 0;
        }       
    }
    class Pierwiastek : Operator
    {
        public Pierwiastek(string Name = "Pierwiastek", int Priority = 3
            , char Symbol='p', int Associativity=(int)Associativity.LEFT) : base(Name, Priority, Symbol, Associativity)
        {
        }
    }
    /********************************************************************/
   
    class InfixNotationTokenizer // Stokenizuj notacje infix ma uporządkowaną listę tokenów
    {
        /*public Queue<Token> TokenizeEquation(string Equation)
        {
            Queue<Token> Tokens = new Queue<Token>();
            for (int i = 0; i < Equation.Length; i++)
            {
                // Iteracja po stringu
                if (Equation[i] == ' ') continue;
                if (Equation[i] == '(') Tokens.Enqueue(new NawiasLewy()); // Token jest zdefiniowany wyżej za pomocą konstruktora z
                // parametrami domyślnymi
                if (Equation[i] == ')') Tokens.Enqueue(new NawiasPrawy());
                if (Equation[i] == '+') Tokens.Enqueue(new Dodawanie());
                if (Equation[i] == '-') Tokens.Enqueue(new Odejmowanie());
                if (Equation[i] == '*') Tokens.Enqueue(new Mnożenie());
                if (Equation[i] == '/') Tokens.Enqueue(new Dzielenie());
                if (Equation[i] == '^') Tokens.Enqueue(new Potęga());
                if (Char.IsNumber(Equation[i]) && i < Equation.Length - 2)
                {
                    string Number = "";
                    Number += Equation[i];
                    for (int j = i + 1; j < Equation.Length; j++)
                    {
                        if (Char.IsNumber(Equation[j]) || Equation[j] == '.')
                        {
                            Number += Equation[j];
                            i = j;
                        }
                        else
                        {
                            break;
                        }
                    }
                    //Tokens.Enqueue(new CyfraCałkowita("CyfraCałkowita", Int32.Parse(Number)));
                    Tokens.Enqueue(new Cyfra("CyfraCałkowita",Int32.Parse(Number)));
                }
                else if (Char.IsNumber(Equation[i]))
                {
                    Tokens.Enqueue(new Cyfra("CyfraCałkowita", Int32.Parse(Char.ToString(Equation[i]))));
                }
                if(Char.IsLetter(Equation[i])) // to jest funkcja  np. sqrt(a)
                {
                    
                    string Func = "";
                    List<Decimal> Params;                   
                    Func += Equation[i];
                    for (int j = i + 1; j < Equation.Length; j++)
                    {
                        if (Char.IsLetter(Equation[j]))
                        {
                            Func += Equation[j]; // Kiedy jest kolejną literą 
                            i = j;
                        }
                        if (Equation[j] == '(')
                        {
                            // zaczynamy czytanie parametrów 
                            while(Equation[j] != ')') // Dopóki nie napotkamy ) co oznacza koniec podawania parametrów
                            {
                                if(Equation[j] == ',') { } // Koniec tego parametru
                            }
                            //i = j;
                        }

                        
                    }
                    //Tokens.Enqueue(new CyfraCałkowita("CyfraCałkowita", Int32.Parse(Number)));
                    Tokens.Enqueue(new Cyfra("CyfraCałkowita", Decimal.Parse(Char.ToString(Equation[i]))));
                }



            }
            return Tokens; // stokenizowana notacja infix na nej wykonamy shunting yard algorithm
        }  */
        
        string Equation;
        public InfixNotationTokenizer(string equation)
        {
            this.Equation = equation;
            TokenizeEquation();
        }
        Queue<Token> InfixNotationTokens;

        public Queue<Token> GetInfixNotationTokens()
        {
            return InfixNotationTokens;
        }

        public void AddSymbol(string equation)
        {
            //Equation += equation;
        }

        public void ClearSymbol()
        {
            /*if(Equation.Length == 1 && Equation != "0")
            {
                Equation = "";
            }*/
        }

        public Queue<Token> TokenizeEquation(bool DevOutput = false) // V2 używa regex 
        {
            Console.WriteLine("RegEx String: " + this);
            Queue<Token> Tokens = new Queue<Token>();
            // string expr = @"(\,|\(|\)|(-?\d*\.?\d+e[+-]?\d+)|\+|\-|\*|\^)|([0-9]+)|sqrt\([\d]\)";
            string expr = @"([A-Z a-z]+\([\d\,?]+\))|(\,|\(|\)|(-?\d*\.?\d+e[+-]?\d+)|\+|\-|\*|\^|\/)|([0-9\.]+)";
            string func = @"([\w]+)(([0-9]*)|([0-9]*))";
            MatchCollection mc = Regex.Matches(this.Equation, expr);
            foreach (Match m in mc)
            {
                Decimal tmp;
                if(m.Value.ToString() == " ") { continue; }
                if(m.Value.ToString().Contains("sqrt")) { Console.Write("SQRT => "); }
                if (m.Value.ToString() == "(") Tokens.Enqueue(new NawiasLewy());
                if (m.Value.ToString() == ")") Tokens.Enqueue(new NawiasPrawy());
                if(m.Value.ToString() == "+") Tokens.Enqueue(new Dodawanie());
                if(m.Value.ToString() == "-") Tokens.Enqueue(new Odejmowanie());
                if(m.Value.ToString() == "*") Tokens.Enqueue(new Mnożenie());
                if(m.Value.ToString() == "/") Tokens.Enqueue(new Dzielenie());
                if(m.Value.ToString() == "^") Tokens.Enqueue(new Potęga());
                if(Decimal.TryParse(m.Value.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out tmp)) Tokens.Enqueue(new Cyfra("Cyfra", tmp));
                if(DevOutput)Console.WriteLine(m);
            }
            this.InfixNotationTokens = Tokens;
            return Tokens;
        }
    }
    class TransformToONP
    {
        // klasa transformująca listę tokenów do poprawnie uporządkowanej listy tokenów zgodnie z notacją ONP za pomocą shunting yard algorithm
        Queue<Token> InfixTokens;
        public Queue<Token> ONP;
        Stack<Token> TokenStack;
        public string ONPstring = "";

        public TransformToONP(Queue<Token> InfixsTokens)
        {
            InfixTokens = InfixsTokens;
            shunting_yard();
        }
        public TransformToONP(string Eq)
        {
            InfixTokens = (new InfixNotationTokenizer(Eq)).TokenizeEquation();
            shunting_yard();
        }
        
        private Boolean shunting_yard()
        {
            ONP = new Queue<Token>();
            TokenStack = new Stack<Token>();
            
                foreach (Token tmp in InfixTokens) {
                    if (tmp is Number) // Liczba
                    {
                        string Typex = tmp.GetType().Name;
                        // To Cyfra Całkowita Yay !
                        // Prześlij na wyjście
                        ONPstring += ((Cyfra)tmp).Value + " ";
                        ONP.Enqueue(tmp);
                    }
                    if (tmp is NawiasLewy) {
                        TokenStack.Push(tmp);
                        continue;
                    }
                    if (tmp is NawiasPrawy) {

                        while (TokenStack.Count() > 0) {
                            Token t = TokenStack.Peek();
                            if (t is NawiasLewy) {
                                TokenStack.Pop();
                                break;
                            }
                            else if (!(t is NawiasLewy)) {
                                // OPERATORY NA WY
                                Token obj = TokenStack.Pop();
                                ONP.Enqueue(obj);
                                ONPstring += ((Operator)obj).Symbol;
                            }
                        }
                        continue;
                    }
                    if (tmp is ArithmeticOperators) {
                        while (TokenStack.Count() > 0) {
                            // Na stosie możemy napotkać nawiasy 
                            // Możemy rzutować bo każdy operator dziedziczy po Operator
                            Operator t = (Operator)TokenStack.Peek();
                            Operator t2 = (Operator)tmp;

                            if ((t.GetPriority() > t2.GetPriority() || t.GetPriority() == t2.GetPriority())) {
                                Operator wy = (Operator)TokenStack.Pop();
                                ONP.Enqueue(wy);
                                ONPstring += wy.Symbol;
                            }
                            else {
                                TokenStack.Push(tmp);
                                break;
                            }
                        }
                        if (TokenStack.Count() == 0) { TokenStack.Push(tmp); };
                        continue;
                    }
                }
            
           
            if(TokenStack.Count() > 0)
            {
                while(TokenStack.Count() > 0)
                {
                    Token t = TokenStack.Pop();
                    ONP.Enqueue(t);
                    ONPstring +=((Operator)t).Symbol;
                }
            }
            return true;
        }
        public void ONPResult()
        {
            Console.WriteLine("Notacja ONP = " + ONPstring);
        }
    }
    class ONP
    {
        
        Stack<Token> HelpStack;
        Queue<Token> ONPTokens;
        
        public ONP(string Equation) // min Liczba , Operator , Liczba np. 2 + 2 , 2 ^ 7 , sin(2) + 3
        {
            InfixNotationTokenizer InfixTokens = new InfixNotationTokenizer(Equation);
            this.ONPTokens = (new TransformToONP(InfixTokens.GetInfixNotationTokens())).ONP;
            HelpStack = new Stack<Token>();
        }
        public ONP(TransformToONP OnpNotation)
        {

        }
        public String ONPCalculationResult()
        {           
            Number ONPresult = new Number("WynikOperacjiArytmetycznej");
            try {
                foreach (Token t in ONPTokens) {
                    if (!(t is Number)) {
                        // Pobierz A i B ze stosu 
                        Number b = (Number)HelpStack.Pop();  // a
                        Number a = (Number)HelpStack.Pop();  // b
                        Number w;
                        ArithmeticOperators op = (ArithmeticOperators)t;
                        w = op.OperatorResult(a, b);
                        HelpStack.Push(w);
                        Console.WriteLine("Wykonuje Operacje na lczbach " + a.Value + " " + op.Symbol + " " + b.Value + " = " + w.Value);

                    }
                    else if (t is Number) {
                        // Prześlij liczbe na stos
                        HelpStack.Push(t);
                    }

                }
            }
            catch(InvalidOperationException e) {
                // error
                return "ERROR";
            }
            ONPresult = (Number)HelpStack.Pop();
            return ONPresult.Value.ToString();
        }

    }
}
