using System.Collections.Generic;

namespace LinqToSqlShared.DbmlObjectModel
{
    public static partial class Dbml
    {
        #region Nested type: PairAssociations

        private class PairAssociations : DbmlVisitor
        {
            private readonly Dictionary<string, List<Association>> associations =
                new Dictionary<string, List<Association>>();

            private readonly Dictionary<Association, Type> associationTypes = new Dictionary<Association, Type>();
            private readonly Dictionary<Type, bool> seen = new Dictionary<Type, bool>();
            private Type currentType;

            private static MatchLevel Compare(Type thisSideType, Association thisSide, Type otherSideType,
                                              Association otherSide)
            {
                MatchLevel noMatch = MatchLevel.NoMatch;
                if (((thisSide.IsForeignKey == true) && (otherSide.IsForeignKey != true)) ||
                    ((thisSide.IsForeignKey != true) && (otherSide.IsForeignKey == true)))
                {
                    noMatch = noMatch;
                }
                if (thisSideType.Name == otherSide.Type)
                {
                    noMatch |= MatchLevel.ThisTypeAgrees;
                }
                if (otherSideType.Name == thisSide.Type)
                {
                    noMatch |= MatchLevel.OtherTypeAgrees;
                }
                if (BuildKeyField(thisSide.GetOtherKey()) == BuildKeyField(otherSide.GetThisKey()))
                {
                    noMatch |= MatchLevel.ThisOtherKeyMatchesOtherThisKey;
                }
                if (BuildKeyField(thisSide.GetThisKey()) == BuildKeyField(otherSide.GetOtherKey()))
                {
                    noMatch |= MatchLevel.ThisThisKeyMatchisOtherOtherKey;
                }
                return noMatch;
            }

            public static Dictionary<Association, Association> Gather(Database db)
            {
                var associations = new PairAssociations();
                associations.VisitDatabase(db);
                var dictionary = new Dictionary<Association, Association>();
                foreach (string str in associations.associations.Keys)
                {
                    Association[] associationArray = associations.associations[str].ToArray();
                    int length = associationArray.Length;
                    int num2 = 0;
                    while (num2 != length)
                    {
                        int index = -1;
                        int num4 = -1;
                        MatchLevel noMatch = MatchLevel.NoMatch;
                        for (int i = 0; i < (length - 1); i++)
                        {
                            for (int j = i + 1; j < length; j++)
                            {
                                Association thisSide = associationArray[i];
                                Association otherSide = associationArray[j];
                                if ((thisSide != null) && (otherSide != null))
                                {
                                    MatchLevel level2 = Compare(associations.associationTypes[thisSide], thisSide,
                                                                associations.associationTypes[otherSide], otherSide);
                                    if (level2 > noMatch)
                                    {
                                        index = i;
                                        num4 = j;
                                        noMatch = level2;
                                    }
                                }
                            }
                        }
                        if ((noMatch & MatchLevel.MinBar) == MatchLevel.MinBar)
                        {
                            Association association3 = associationArray[index];
                            Association association4 = associationArray[num4];
                            dictionary[association3] = association4;
                            dictionary[association4] = association3;
                            associationArray[index] = null;
                            associationArray[num4] = null;
                            num2 += 2;
                        }
                        else
                        {
                            foreach (Association association5 in associationArray)
                            {
                                if (association5 != null)
                                {
                                    dictionary[association5] = null;
                                    num2++;
                                }
                            }
                        }
                    }
                }
                return dictionary;
            }

            public override Association VisitAssociation(Association association)
            {
                if (!associations.ContainsKey(association.Name))
                {
                    associations[association.Name] = new List<Association>();
                }
                associations[association.Name].Add(association);
                associationTypes[association] = currentType;
                return base.VisitAssociation(association);
            }

            public override Type VisitType(Type type)
            {
                Type type3;
                Type type2 = type;
                try
                {
                    currentType = type;
                    if (seen.ContainsKey(type))
                    {
                        return type;
                    }
                    seen.Add(type, true);
                    type3 = base.VisitType(type);
                }
                finally
                {
                    currentType = type2;
                }
                return type3;
            }

            #region Nested type: MatchLevel

            private enum MatchLevel
            {
                ForeignNonForeign = 4,
                MinBar = 3,
                NoMatch = 0,
                OtherTypeAgrees = 2,
                ThisOtherKeyMatchesOtherThisKey = 8,
                ThisThisKeyMatchisOtherOtherKey = 0x10,
                ThisTypeAgrees = 1
            }

            #endregion
        }

        #endregion
    }
}