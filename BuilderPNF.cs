using System;
using System.Text;
using System.Collections.Generic;

namespace lab
{
    public class BuilderPNF
    {
        public event EventHandler<BuildNormalFormEventArgs> EventBuildСonjunct;
        public event EventHandler<BuildNormalFormEventArgs> EventBuildDisjoint;

        private List<KeyValuePair<string, int>> _truthTable;
        private SortedDictionary<char, int> _listVariables;
        private readonly string _formula;

        public BuilderPNF(string input)
        {
            if (input == null)
            {
                _formula = "";
            }
            else
            {
                _formula = input;
                _truthTable = BuildTruthTable();
            }
        }

        public char[] ListVariables
        {
            get
            {
                var list = new char[_listVariables.Count];

                var i = 0;
                foreach (var item in _listVariables)
                    list[i++] = item.Key;

                return list;
            }
        }

        public string BuildPDNF()
        {
            if (_truthTable.Count == 1 && _truthTable[0].Key == string.Empty)
                return null;

            var result = new StringBuilder();

            foreach(var item in _truthTable)
            {
                if (item.Value != 1) 
                    continue;
                var listForEvent = new List<string>();
                result.Append('(');

                var i = 0;
                foreach(var let in _listVariables)
                {
                    if (item.Key[i] == '1')
                    {
                        result.Append(@let.Key + " & ");
                        listForEvent.Add(@let.Key.ToString());
                    }
                    else
                    {
                        result.Append("!" + @let.Key + " & ");
                        listForEvent.Add("!"+@let.Key.ToString());
                    }
                    i++;
                }
                result.Remove(result.Length - 3, 3);
                result.Append(") | ");


                var args = new BuildNormalFormEventArgs(listForEvent);
                EventBuildDisjoint?.Invoke(this, args);
            }

            if (result.Length == 0) 
                return null;

            result.Remove(result.Length - 3, 3);

            return result.ToString();
        }

        public string BuildPKNF()
        {
            if (_truthTable.Count == 1 && _truthTable[0].Key == string.Empty)
                return null;

            var result = new StringBuilder();
            
            foreach (var item in _truthTable)
            {
                if (item.Value != 0) 
                    continue;
                var listForEvent = new List<string>();

                result.Append('(');
                var i = 0;
                foreach (var let in _listVariables)
                {
                    if (item.Key[i] == '0')
                    {
                        result.Append(@let.Key + " | ");
                        listForEvent.Add(@let.Key.ToString());
                    }
                    else
                    {
                        result.Append("!" + @let.Key + " | ");
                        listForEvent.Add("!"+@let.Key);
                    }
                    i++;
                }
                result.Remove(result.Length - 3, 3);
                result.Append(") & ");


                var args = new BuildNormalFormEventArgs(listForEvent);
                EventBuildСonjunct?.Invoke(this, args);
            }

            if (result.Length == 0) 
                return null ;

            result.Remove(result.Length - 3, 3);

            return result.ToString();
        }

        public List<KeyValuePair<string, int>> BuildTruthTable()
        {
            _listVariables = new SortedDictionary<char, int>();
                
            foreach (var item in _formula)
                if (char.IsLetter(item) && !_listVariables.ContainsKey(item))
                    _listVariables.Add(item, 0);

            _truthTable = new List<KeyValuePair<string, int>>();

            if (_listVariables.Count == 0)
            {
                var result = RPN.Calculate(_formula);
                _truthTable.Add(new KeyValuePair<string, int>(string.Empty, result));
                
                return _truthTable;
            }

            for (var i = 0; i < (int)Math.Pow(2, _listVariables.Count); i++)
                _truthTable.Add(new KeyValuePair<string, int>(new string('0',_listVariables.Count - Convert.ToString(i, 2).Length) + Convert.ToString(i, 2),
                    RPN.Calculate(ReplaceLets(_formula, i))));

            return _truthTable;
        }

        private string ReplaceLets(string str, int code)
        {
            var result = new StringBuilder();

            var strCode = new string('0', _listVariables.Count - Convert.ToString(code, 2).Length) + Convert.ToString(code, 2);

            var i = 0;
            var keys = new List<char>(_listVariables.Keys);
            foreach (var let in keys)
                _listVariables[let] = strCode[i++] - '0';

            foreach (var item in str)
            {
                if(_listVariables.ContainsKey(item))
                    result.Append(_listVariables[item]);
                else
                    result.Append(item);
            }

            return result.ToString();
        }
    }
}
