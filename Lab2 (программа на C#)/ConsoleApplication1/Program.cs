using System;
using System.Collections.Generic;

namespace Program
{
	class Program
    {
        private static uint[,] adjacencyMatrix;
        private const int componentsCount = 17;
        private const int sizeGroupA = 6;
        private const int sizeGroupB = 5;
        private const int sizeGroupC = 6;
        private static List<uint> groupA;
        private static List<uint> groupB;
        private static List<uint> groupC;
        private static List<uint> unusedElements;

        static void Main(string[] args)
        {
			// задаем матрицу смежности
			adjacencyMatrix = new uint[componentsCount, componentsCount] 
            {
                {0,1,1,0,0,0,1,0,1,0,0,0,0,0,0,0,0},
                {1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
                {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {0,1,0,0,1,1,1,0,1,1,0,0,0,1,0,0,0},
                {0,1,0,1,0,1,1,0,1,1,0,0,0,1,0,0,0},                
                {0,1,0,1,1,0,1,0,0,0,0,0,0,0,0,0,1},
                {1,1,0,1,1,1,0,0,1,0,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,1,0,1,1,0,0,0,1},
                {1,0,0,1,1,0,1,1,0,1,0,0,0,1,0,0,0},
                {0,0,0,1,1,0,0,1,1,0,0,1,1,1,0,0,0},
                {0,0,0,0,0,0,1,0,0,0,0,1,1,0,1,1,0},
                {0,0,0,0,0,0,0,1,0,1,1,0,1,0,1,1,0},
                {0,0,0,0,0,0,0,1,0,1,1,1,0,0,1,1,0},
                {0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0,1},
                {0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,1,1},
                {0,0,0,0,0,0,0,0,0,0,1,1,1,0,1,0,1},
                {0,0,1,0,0,1,0,1,0,0,0,0,0,1,1,1,0}
            };
            // задаем неиспользованные элементы
            unusedElements = new List<uint>();

            uint[] edgeCount = new uint[componentsCount];
            Console.Write("Матрица смежности:\r\n\n№\tСоединения\r\n");
            for (int i = 0; i < componentsCount; i++)
            {
                unusedElements.Add((uint)i);
                uint currentRow = 0;
                Console.Write(i.ToString() + "\t");
                for (int j = 0; j < componentsCount; j++)
                {
                    currentRow += adjacencyMatrix[i, j];
                    Console.Write(adjacencyMatrix[i, j].ToString() + " ");
                }
                edgeCount[i] = currentRow;
                Console.Write("   " + currentRow.ToString() + "\r\n");
            }

            groupA = createGroup(sizeGroupA);

            Console.Write("Группа А создана из компонентов №");
            for (int i = 0; i < groupA.Count; i++)
            {
                Console.Write(" " + groupA[i].ToString());
            }
            groupB = createGroup(sizeGroupB);
            Console.Write("\r\nГруппа B создана из компонентов №");
            for (int i = 0; i < groupB.Count; i++)
            {
                Console.Write(" " + groupB[i].ToString());
            }
            groupC = createGroup(sizeGroupC);
            Console.Write("\r\nГруппа C создана из компонентов №");
            for (int i = 0; i < groupC.Count; i++)
            {
                Console.Write(" " + groupC[i].ToString());
            }
            Console.Write("\r\n\n");
			Console.WriteLine("\r\nНажмите любую клавишу для выхода.");
			Console.ReadKey();
        }

        private static List<uint> createGroup(uint size)
        {
            List<uint> result = new List<uint>();
            uint minElement = unusedElements[0];
            int minEdgeValue = -1;
            // находим вершину графа с наименьшей локальной степенью (с оставшимися элементами)
            for (int i = 0; i < unusedElements.Count; i++)
            {
                uint currentElement = unusedElements[i];
                int currentEdgeValue = 0;
                for (int k = 0; k < componentsCount; k++)
                {
                    if (unusedElements.IndexOf((uint)k) != -1)
                    {
                        if (adjacencyMatrix[currentElement, k] > 0)
                        {
                            currentEdgeValue++;
                        }
                    }
                }
                if (minEdgeValue == -1 || minEdgeValue > currentEdgeValue)
                {
                    minElement = currentElement;
                    minEdgeValue = currentEdgeValue;
                }
            }

            if (size == 1)
            {
                result.Add(minElement);
                return result;
            }
            // проверяем, вместятся ли его соседи в группу с ним
            if (minEdgeValue + 1 == size || unusedElements.Count <= size)
            {
                if (unusedElements.Count > size)
                {
                    // перебор в обратном порядке, дабы ничего не нарушить ;)
                    for (int i = unusedElements.Count - 1; i >= 0; i--)
                    {
                        uint currentElement = unusedElements[i];
                        if (adjacencyMatrix[minElement, currentElement] > 0 || currentElement == minElement)
                        {
                            result.Add(currentElement);
                            unusedElements.Remove(currentElement);
                        }
                    }
                    // группа составлена
                }
                else
                {
                    result = unusedElements;
                }
            }
            else
            {
                // если размер полученной группы меньше, чем желаемый
                if (minEdgeValue + 1 < size)
                {
                    // добавляем соседей в результат
                    for (int i = unusedElements.Count - 1; i >= 0; i--)
                    {
                        uint currentElement = unusedElements[i];
                        if (adjacencyMatrix[minElement, currentElement] > 0 || minElement == currentElement)
                        {
                            result.Add(currentElement);
                            unusedElements.Remove(currentElement);
                        }
                    }
                    List<uint> tempList = createGroup(size - (uint)result.Count);
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        result.Add(tempList[i]);
                    }
                }
                // иначе, если больше
                else
                {
                    // попробуем так - добавим в группу все связанные элементы
                    // и выкинем те, которые имеют наибольшие связи с внешними компонентами
                    // заполняемгруппусначала
                    for (int i = unusedElements.Count - 1; i >= 0; i--)
                    {
                        uint currentElement = unusedElements[i];
                        if (currentElement == minElement || adjacencyMatrix[minElement, currentElement] > 0)
                        {
                            result.Add(currentElement);
                            unusedElements.Remove(currentElement);
                        }
                    }
                    // группа составлена, теперь необходимо исключить элементы
                    int constParam = result.Count - (int)size;
                    for (int i = 0; i < constParam; i++)
                    {
                        uint valToRemove = result[0];
                        int delta = 0;
                        for (int k = result.Count - 1; k >= 0; k--)
                        {
                            int innerConnections = 0;
                            int outerConnections = 0;
                            uint currentComponent = result[k];
                            for (int l = 0; l < componentsCount; l++)
                            {
                                if (result.IndexOf((uint)l) != -1)
                                {
                                    innerConnections += (int)adjacencyMatrix[currentComponent, l];
                                }
                                else
                                {
                                    outerConnections += (int)adjacencyMatrix[currentComponent, l];
                                }
                            }

                            int newDelta = outerConnections - innerConnections;

                            if (k == result.Count - 1)
                            {
                                delta = newDelta;
                            }
                            else
                            {
                                if (newDelta < delta)
                                {
                                    delta = newDelta;
                                    valToRemove = result[k];
                                }
                            }
                        }

                        result.Remove(valToRemove);
                        unusedElements.Add(valToRemove);
                        unusedElements.Sort();
                        if (result.Count == size)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
