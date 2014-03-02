
/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013 - 2014>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*////////////////////////////////////////////////////////////////////

﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EasyFarm.PlayerTools
{
    /// <summary>
    /// An action to be used on a target unit or player.
    /// Could be a spell or an ability.
    /// </summary>
    public class Ability
    {
        public int ID, Index, MPCost, TPCost;
        public double CastTime, Recast;
        public string Prefix, Name, Type, Element, Targets, Skill, Alias;
        public string Postfix = "";
        public bool IsValidName { get { return !string.IsNullOrWhiteSpace(Name); } }
        public bool IsSpell, IsAbility;

        private const string abils = "abils.xml";
        private const string spells = "spells.xml";
        private static XElement SpellsDoc = null;
        private static XElement AbilsDoc = null;

        /// <summary>
        /// Class load time initializer
        /// </summary>
        static Ability()
        {
            AbilsDoc = LoadResource(abils);
            SpellsDoc = LoadResource(spells);
        }

        /// <summary>
        /// Creates a blank ability.
        /// </summary>
        public Ability() { }

        /// <summary>
        /// Creates an ability by name and sets it's 
        /// IsAbility and IsSpell Fields.
        /// </summary>
        /// <param name="name"></param>
        public Ability(string name)
        {
            Ability Ability = CreateAbility(name);

            this.ID = Ability.ID;
            this.Index = Ability.Index;
            this.MPCost = Ability.MPCost;
            this.TPCost = Ability.TPCost;
            this.CastTime = Ability.CastTime;
            this.Recast = Ability.Recast;
            this.Prefix = Ability.Prefix;
            this.Name = Ability.Name;
            this.Type = Ability.Type;
            this.Element = Ability.Element;
            this.Targets = Ability.Targets;
            this.Skill = Ability.Skill;
            this.Alias = Ability.Alias;
            this.Postfix = Ability.Postfix;
            this.IsSpell = Ability.IsSpell;
            this.IsAbility = Ability.IsAbility;
        }

        /// <summary>
        /// Creates an ability obj. This object may be a spell
        /// or an ability.
        /// </summary>
        /// <param name="name">Ability's Name</param>
        /// <returns>a new ability</returns>
        private static Ability CreateAbility(string name)
        {
            var JobAbility = ParseAbilityXML(name, AbilsDoc);
            var MagicAbility = ParseSpellXML(name, SpellsDoc);
            return JobAbility.IsValidName ? JobAbility : MagicAbility;
        }

        /// <summary>
        /// Parses a resource in terms of an ability.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="XMLDoc"></param>
        /// <returns></returns>
        private static Ability ParseAbilityXML(string name, XElement XMLDoc)
        {
            // Fetches the ability from xml.
            XElement element = XMLDoc.Elements("a").Attributes().Where(x => x.Name == "english" && x.Value == name).Select(x => x.Parent).SingleOrDefault();

            foreach (XElement elm in XMLDoc.Elements("a").Attributes().Select(x=> x.Parent))
            {
                if (elm.Attribute("english").Equals("Caliginosity"))
                    Console.WriteLine(elm.Attribute("english"));
            }

            // Return blank if we did not find the ability.
            if (element == null)
            {
                return new Ability();
            }

            // Create a new ability from attributes in move.
            return new Ability()
            {
                Alias = (string)element.Attribute("alias"),                
                Element = (string)element.Attribute("element"),
                Name = (string)element.Attribute("english"),
                Prefix = (string)element.Attribute("prefix"),
                Skill = (string)element.Attribute("skill"),
                Targets = (string)element.Attribute("targets"),
                Type = (string)element.Attribute("type"),
                CastTime = (double)element.Attribute("casttime"),
                ID = (int)element.Attribute("id"),
                Index = (int)element.Attribute("index"),
                MPCost = (int)element.Attribute("mpcost"),
                Recast = (double)element.Attribute("recast"),
                TPCost = (int)element.Attribute("tpcost"),
                IsAbility = true
            };
        }

        /// <summary>
        /// Parses a resource in terms of an spell.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="XMLDoc"></param>
        /// <returns></returns>
        /// 

        /* Sleepga II matches twice, fix*/
        private static Ability ParseSpellXML(string name, XElement XMLDoc)
        {
            // Fetches the ability from xml.
            XElement element = XMLDoc.Elements("s").Attributes().Where(x => (x.Name == "english" && x.Value == name)).Select(x => x.Parent).SingleOrDefault();

            // Return blank if we did not find the ability.
            if (element == null) { return new Ability(); }

            // Create a new ability from attributes in move.
            return new Ability()
            {
                Alias = (string)element.Attribute("alias"),
                Element = (string)element.Attribute("element"),
                Name = (string)element.Attribute("english"),
                Prefix = (string)element.Attribute("prefix"),
                Skill = (string)element.Attribute("skill"),
                Targets = (string)element.Attribute("targets"),
                Type = (string)element.Attribute("type"),
                CastTime = (double)element.Attribute("casttime"),
                ID = (int)element.Attribute("id"),
                Index = (int)element.Attribute("index"),
                MPCost = (int)element.Attribute("mpcost"),
                Recast = (double)element.Attribute("recast"),
                IsSpell = true
            };
        }

        /// <summary>
        /// Ensures that the resource file passed exists
        /// and returns the XElement obj associated with the file.
        /// </summary>
        /// <param name="abils"></param>
        /// <returns></returns>
        private static XElement LoadResource(string abils)
        {
            String WorkingDirectory = Directory.GetCurrentDirectory();

            if (Directory.Exists("resources"))
            {
                Directory.SetCurrentDirectory("resources");
            }

            if (!File.Exists(abils))
            {
                return null;
            }

            XElement XMLDoc = XElement.Load(abils);

            Directory.SetCurrentDirectory(WorkingDirectory);

            return XMLDoc;
        }

        /// <summary>
        /// Returns the command to execute the ability or spell
        ///      /ma "Dia" <t>
        ///      /ja "Provoke" <t>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // If it was intended to work on use, 
            // set it to cast on us
            if (Targets.ToLower().Contains("self"))
                Postfix = "<me>";
            else if (Targets.ToLower().Contains("enemy"))
                Postfix = "<t>";

            // If it was a ranged attack, use the ranged attack syntax
            if (Prefix == "/range")
                return Prefix + " " + Postfix;
            // Use the spell/ability syntax.
            else
                return Prefix + " \"" + Name + "\" " + Postfix;
        }
    }
}