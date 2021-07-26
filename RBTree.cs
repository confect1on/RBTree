using System;
using System.Collections;
using System.Collections.Generic;

namespace Red_Black_Tree
{
    /// <summary>
    /// Класс, описывающий шаблон красно-черного дерева, реализующий интерфейс IEnumerable для перечисления узлов красно-черного дерева
    /// </summary>
    /// <typeparam name="T">Произвольный тип T, реализующий интерфейс IComparable, так как все ключи в узлах красно-черного дерева должны быть сравнимы между собой</typeparam>
    public class RBTree<T> : IEnumerable<RBTreeNode<T>> where T : IComparable
    {
        #region Constructor
        
        /// <summary>
        /// Конструктор, инициализирующий красно-черное дерево
        /// </summary>
        public RBTree()
        {
            _root = RBTreeNode<T>.CreateLeaf();
        }

        #endregion
        
        #region Count
        
        /// <summary>
        /// Авто-свойство для работы с количеством узлов в красно-черном дереве
        /// </summary>
        public int Count { get; private set; }
        
        #endregion

        #region Root

        private RBTreeNode<T> _root; //поле, хранящее в себе ссылку на корневой узел красно-черного дерева
        
        /// <summary>
        /// Авто-свойство для работы с корнем красно-черного дерева
        /// </summary>
        public RBTreeNode<T> Root
        {
            get => _root;
            private set
            {
                _root = value;
                _root.Parent = RBTreeNode<T>.CreateLeaf();
            }
        }

        #endregion

        #region Insert
        
        /// <summary>
        /// Метод, реализующий алгоритм вставки нового узла с данным ключом в красно-черное дерево
        /// </summary>
        /// <param name="value">Ключ</param>
        public void Insert(T value)
        {
            var current = Root;
            var parent = RBTreeNode<T>.CreateLeaf();
            
            while (!current.IsLeaf) //находим место для вставки
            {
                parent = current;
                current = value.CompareTo(current.Value) < 0 ? current.Left : current.Right;
            }

            var insertNode = new RBTreeNode<T>(value, NodeColor.Red);
            
            if (parent.IsLeaf) //если вставляем в корень
            {
                Root = insertNode;
                current = Root;
            }
            else if (insertNode.Value.CompareTo(parent.Value) < 0) //если вставляем в левое поддерево
            {
                parent.Left = insertNode;
                current = parent.Left;
            }
            else //если вставляем в правое поддерево
            {
                parent.Right = insertNode;
                current = parent.Right;
            }
            
            //создаем Null-потомков у вставленного узла
            insertNode.Left = RBTreeNode<T>.CreateLeaf();
            insertNode.Right = RBTreeNode<T>.CreateLeaf();
            
            InsertBalance(current); //вызываем метод ребалансировки после вставки

            Count++; //увеличиваем количество узлов в красно-черном дереве
        }
        
        /// <summary>
        /// Метод, реализующий балансировку красно-черного дерева после вставки
        /// </summary>
        /// <param name="z">Узел, вставленный в дерево</param>
        private void InsertBalance(RBTreeNode<T> z)
        {
            //при вставке могли нарушится два свойства красно-черного дерева из-за вставки красного узла
            //свойство 2: корень стал красным
            //свойство 4: красный узел имеет красного потомка
            while (z.Parent.Color == NodeColor.Red) //пока отец красный
            {
                if (z.Parent == z.Grandparent.Left) //отец - левый потомок дедушки
                {
                    var y = z.Grandparent.Right; //brother of parent
                    if (y.Color == NodeColor.Red) //случай 1: узел y красный
                    {
                        z.Parent.Color = NodeColor.Black;
                        y.Color = NodeColor.Black;
                        z.Grandparent.Color = NodeColor.Red;
                        z = z.Grandparent;
                    }
                    else
                    {
                        if (z == z.Parent.Right) //случай 2: узел y черный и z - правый потомок
                        {
                            z = z.Parent;
                            LeftRotation(z);
                        }
                        //случай 3: узел y черный и z - левый потомок
                        z.Parent.Color = NodeColor.Black;
                        z.Grandparent.Color = NodeColor.Red;
                        RightRotation(z.Grandparent);
                    }
                }
                else //отец - правый потомок дедушки
                {
                    var y = z.Grandparent.Left; //brother of parent
                    if (y.Color == NodeColor.Red) //случай 1: узел y - красный
                    {
                        z.Parent.Color = NodeColor.Black;
                        y.Color = NodeColor.Black;
                        z.Grandparent.Color = NodeColor.Red;
                        z = z.Grandparent;
                    }
                    else
                    {
                        if (z == z.Parent.Left) //случай 2: узел y черный и z - левый потомок
                        {
                            z = z.Parent;
                            RightRotation(z);
                        }
                        //случай 3: узел y черный и z - правый потомок
                        z.Parent.Color = NodeColor.Black;
                        z.Grandparent.Color = NodeColor.Red;
                        LeftRotation(z.Grandparent);
                    }
                }
            }

            Root.Color = NodeColor.Black; //восстанавливаем черноту корня
        }

        #endregion

        #region Contains
        /// <summary>
        /// Метод, проверяющий, имеется ли данный ключ в дереве
        /// </summary>
        /// <param name="value">Ключ</param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            return Search(value) != null;
        }
        
        /// <summary>
        /// Метод, реализующий алгоритм поиска в бин. дереве. Возвращает узел с данным нам ключом.
        /// Если узел с искомым ключом отсутствует, то возвращает null.
        /// </summary>
        /// <param name="value">Ключ</param>
        /// <returns></returns>
        private RBTreeNode<T> Search(T value)
        {
            var current = Root;
            while (current != null)
            {
                if (value.CompareTo(current.Value) < 0) current = current.Left;
                else if (value.CompareTo(current.Value) > 0) current = current.Right;
                else if (value.CompareTo(current.Value) == 0) return current;
            }

            return null;
        }

        #endregion
        
        #region Delete

        /// <summary>
        /// Метод, реализующий алгоритм удаления узла из красно-черного дерева.
        /// </summary>
        /// <param name="value">Ключ</param>
        /// <exception cref="InvalidOperationException">Узел с искомым ключом отсутствует.</exception>
        public void Delete(T value)
        {
            if (!Contains(value)) throw new InvalidOperationException($"Value {value} is missing in the tree");

            var currentNode = Search(value);
            RBTreeNode<T> childNode = null;

            //случай 1: у удаляемого узла нет потомков
            if (currentNode.Left.IsLeaf && currentNode.Right.IsLeaf)
            {
                if (currentNode == Root)
                {
                    Root = RBTreeNode<T>.CreateLeaf();
                    Count--;
                    return;
                }

                if (currentNode.Parent.Left == currentNode)
                {
                    currentNode.Parent.Left = RBTreeNode<T>.CreateLeaf();
                    childNode = currentNode.Parent.Left;
                }

                else if (currentNode.Parent.Right == currentNode)
                {
                    currentNode.Parent.Right = RBTreeNode<T>.CreateLeaf();
                    childNode = currentNode.Parent.Right;
                }

                

            }
            //случай 2: у удаляемого узла один потомок
            else if (currentNode.Left.IsLeaf && !currentNode.Right.IsLeaf || 
                     !currentNode.Left.IsLeaf && currentNode.Right.IsLeaf)
            {
                //2.1)1 rebenok - sprava
                if (currentNode.Left.IsLeaf && !currentNode.Right.IsLeaf)
                {

                    if (currentNode == Root)
                    {
                        Root = currentNode.Right;
                        childNode = Root;
                    }

                    else
                    {

                        if (currentNode.Parent.Left == currentNode)
                        {
                            currentNode.Parent.Left = currentNode.Right;
                            childNode = currentNode.Parent.Left;
                        }

                        else if (currentNode.Parent.Right == currentNode)
                        {
                            currentNode.Parent.Right = currentNode.Right;
                            childNode = currentNode.Parent.Right;
                        }

                    }
                }

                //2.2) 1 rebenok - sleva
                else if (!currentNode.Left.IsLeaf && currentNode.Right.IsLeaf)
                {

                    if (currentNode == Root)
                    {
                        Root = currentNode.Left;
                        childNode = Root;
                    }

                    else
                    {

                        if (currentNode.Parent.Left == currentNode)
                        {
                            currentNode.Parent.Left = currentNode.Left;
                            childNode = currentNode.Parent.Left;
                        }

                        else if (currentNode.Parent.Right == currentNode)
                        {
                            currentNode.Parent.Right = currentNode.Left;
                            childNode = currentNode.Parent.Right;
                        }

                    }
                }
            }

            //случай 3: у удаляемого узла два потомка
            else if (!currentNode.Left.IsLeaf && !currentNode.Right.IsLeaf) 
            {
                var smallestInRight = currentNode.Right;

                while (!smallestInRight.Left.IsLeaf)
                {
                    smallestInRight = smallestInRight.Left;
                }

                if (!smallestInRight.Right.IsLeaf)
                {
                    smallestInRight.Right.Parent = smallestInRight.Parent;
                }
                

                if (smallestInRight == smallestInRight.Parent.Left)
                {
                    smallestInRight.Parent.Left = smallestInRight.Right;
                    childNode = smallestInRight.Parent.Left;
                }
                else if (smallestInRight == smallestInRight.Parent.Right)
                {
                    smallestInRight.Parent.Right = smallestInRight.Right;
                    childNode = smallestInRight.Parent.Right;
                }

                currentNode.Value = smallestInRight.Value;

                currentNode = smallestInRight;

            }

            //удаление красного узла не повлечет нарушения свойств
            //необходима проверка на черноту удаляемого
            if (currentNode.Color == NodeColor.Black)
            {
                DeleteBalance(childNode); //вызываем метод балансировки красно-черного дерева от потомка удаленного узла
            }

            Count--; //уменьшаем количество узлов в дереве
        }
        
        /// <summary>
        /// Метод, реализующий алгоритм балансировки красно-черного дерева после удаления черного узла
        /// </summary>
        /// <param name="x">Потомок удаленного узла</param>
        private void DeleteBalance(RBTreeNode<T> x)
        {
            while (x != Root && x.Color == NodeColor.Black)
            {
                if (x == x.Parent.Left) //если узел левый потомок
                {
                    var w = x.Sibling;
                    if (w.Color == NodeColor.Red) //случай 1: узел w красный
                    {
                        w.Color = NodeColor.Black;
                        x.Parent.Color = NodeColor.Red;
                        LeftRotation(x.Parent);
                        w = x.Sibling;
                    }

                    //случай 2: узел w черный и оба его потомка черные
                    if (w.Left.Color == NodeColor.Black && w.Right.Color == NodeColor.Black)
                    {
                        w.Color = NodeColor.Red;
                        x = x.Parent;
                    }
                    else
                    {
                        //случай 3: узел w черный, правый потомок черный, левый потомок красный
                        if (w.Right.Color == NodeColor.Black)
                        {
                            w.Color = NodeColor.Red;
                            w.Left.Color = NodeColor.Black;
                            RightRotation(w);
                            w = x.Parent.Right;
                        }
                        //случай 4: узел w черный, правый потомок красный, левый потомок черный
                        w.Color = x.Parent.Color;
                        x.Parent.Color = NodeColor.Black;
                        w.Right.Color = NodeColor.Black;
                        LeftRotation(x.Parent);
                        x = Root;
                    }
                }

                else if (x == x.Parent.Right) //если узел правый потомок
                {
                    var w = x.Sibling;
                    if (w.Color == NodeColor.Red) //случай 1: узел w красный
                    {
                        w.Color = NodeColor.Black;
                        x.Parent.Color = NodeColor.Red;
                        RightRotation(x.Parent);
                        w = x.Parent.Left;
                    }

                    //случай 2: узел w черный, оба потомка черные
                    if (w.Left.Color == NodeColor.Black && w.Right.Color == NodeColor.Black)
                    {
                        w.Color = NodeColor.Red;
                        x = x.Parent;
                    }

                    else
                    {
                        if (w.Left.Color == NodeColor.Black) //случай 3: узел w черный, правый потомок красный, левый потомок черный
                        {
                            w.Color = NodeColor.Red;
                            w.Right.Color = NodeColor.Black;
                            LeftRotation(w);
                            w = x.Sibling;
                        }
                        //случай 4: узел w черный, правый потомок черный, левый потомок красный
                        w.Color = x.Parent.Color;
                        x.Parent.Color = NodeColor.Black;
                        w.Left.Color = NodeColor.Black;
                        RightRotation(x.Parent);
                        x = Root;
                    }
                }
            }
            
            //выполняем перекраску
            x.Color = NodeColor.Black;
            Root.Color = NodeColor.Black;
        }

        #endregion

        #region IEnumerable implementation
        
        /// <summary>
        /// Метод, реализующий прямой обход бинарного дерева, возвращающий перечисление последовательности узлов в отсортированном порядке.
        /// Используется итеративный алгоритм на стеке.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RBTreeNode<T>> InorderTraversal()
        {
            if (Root.IsLeaf) yield break; //если корень является листом, оборвать последовательность
            var stack1 = new Stack<RBTreeNode<T>>();

            stack1.Push(Root);
            while (stack1.Count > 0)
            {
                var temp = stack1.Pop();
                if (!temp.IsLeaf) yield return temp;

                if (temp.Left != null) stack1.Push(temp.Left);

                if (temp.Right != null) stack1.Push(temp.Right);
            }
            
        }
        
        /// <summary>
        /// Возвращает итератор последовательности узлов, полученных при обходе
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RBTreeNode<T>> GetEnumerator()
        {
            return InorderTraversal().GetEnumerator();
        }
        
        /// <summary>
        /// Явная реализация метода GetEnumerator() интерфейса IEnumerable
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        
        #region Rotations
        
        /// <summary>
        /// Метод, реализующий алгоритм малого правого поворота относительно узла x.
        /// </summary>
        /// <param name="x">Узел, относительно которого совершаем поворот.</param>
        internal void RightRotation(RBTreeNode<T> x)
        {
            var y = x.Left;
            ReplaceCurrentRoot(y, x);
            x.Left = y.Right;
            y.Right = x;
        }
        
        /// <summary>
        /// Метод, реализующий алгоритм малого левого поворота относительно узла x.
        /// </summary>
        /// <param name="x">Узел, относительно которого выполняется левый поворот.</param>
        internal void LeftRotation(RBTreeNode<T> x)
        {
            var y = x.Right;
            ReplaceCurrentRoot(y, x);
            x.Right = y.Left;
            y.Left = x;
        }

        /// <summary>
        /// Метод, реализующий замену корня в поддереве, элементами которого являются x и y
        /// </summary>
        /// <param name="y">Новый корень</param>
        /// <param name="x">Старый корень</param>
        private void ReplaceCurrentRoot(RBTreeNode<T> y, RBTreeNode<T> x)
        {
            if (x == Root) //если x является корнем
            {
                Root = y; //новым корнем становится y
            }
            else //если x не является корнем
            {
                if (x.Parent.Left == x) //если x является правым потомком
                {
                    x.Parent.Left = y;
                }
                else if (x.Parent.Right == x) //если x является левым потомком
                {
                    x.Parent.Right = y;
                }
            }
            y.Parent = x.Parent; //родителем y становится родитель x
            x.Parent = y; //родителем x становится y
        }
        
        #endregion

        #region Height

        /// <summary>
        /// Метод, возвращающий максимальную высоту в дереве
        /// </summary>
        /// <returns></returns>
        public int GetMaxHeight()
        {
            return GetMaxHeight(_root);
        }
        
        /// <summary>
        /// Метод, реализующий алгоритм рекурсивного поиска максимальной высоту
        /// </summary>
        /// <param name="rbTreeNode">Узел, от которого будем вести поиск</param>
        /// <returns></returns>
        public static int GetMaxHeight(RBTreeNode<T> rbTreeNode)
        {
            if (rbTreeNode.IsLeaf) return 0;
            return 1 + Math.Max(GetMaxHeight(rbTreeNode.Left), GetMaxHeight(rbTreeNode.Right)); //возвращаем максимальное из высот поддеревьев
        }

        #endregion
        
    }
}