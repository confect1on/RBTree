using System;

namespace Red_Black_Tree
{
    /// <summary>
    /// Класс, описывающий шаблон для узлов красно-черного дерева
    /// </summary>
    /// <typeparam name="T">Произвольный тип T, реализующий интерфейс IComparable, необходимый потому, что все значения узлов красно-черного дерева должны быть сравнимы между собой</typeparam>
    public class RBTreeNode<T> where T : IComparable
    {
        #region Fields
        
        public readonly bool IsLeaf; //поле, хранящение информацию о том, является ли лист узлом

        #endregion

        #region Constructor
        
        /// <summary>
        /// Конструктор, инициализирующий листовой(null-узел) узел красно-черного дерева
        /// </summary>
        private RBTreeNode()
        {
            Value = default;
            IsLeaf = true;
            Color = NodeColor.Black;
        }
        
        /// <summary>
        /// Конструктор, инициализирующий узел красно-черного дерева
        /// </summary>
        /// <param name="value">Значение узла</param>
        /// <param name="color">Цвет узла</param>
        public RBTreeNode(T value, NodeColor color)
        {
            Value = value;
            IsLeaf = false;
            Color = color;
        }

        #endregion

        #region Leaf
        /// <summary>
        /// Статический метод, возвращающий новый листовой узел
        /// </summary>
        internal static RBTreeNode<T> CreateLeaf()
        {
            return new RBTreeNode<T>();
        }
        
        #endregion

        #region Right
        
        private RBTreeNode<T> _right; //поле с ссылкой на правого потомка
        
        /// <summary>
        /// Свойство для работы с правым потомком узла
        /// </summary>
        public RBTreeNode<T> Right
        {
            get => _right;
            internal set
            {
                _right = value.IsLeaf 
                    ? CreateLeaf() 
                    : value;

                if (_right != null) _right.Parent = this;
            }
        }

        #endregion

        #region Left

        private RBTreeNode<T> _left; //поле с ссылкой на левого потомка узла
        
        /// <summary>
        /// Свойство для работы с левым потомком узла
        /// </summary>
        public RBTreeNode<T> Left
        {
            get => _left;
            internal set
            {
                _left = value.IsLeaf
                    ? CreateLeaf()
                    : value;
                if (_left != null) _left.Parent = this;
            }
        }

        #endregion
        
        #region Parent
        
        /// <summary>
        /// Авто-свойство для работы с родителем узла
        /// </summary>
        public RBTreeNode<T> Parent { get; internal set; }
        
        #endregion

        #region Grandparent
        
        /// <summary>
        /// get-only свойство, возвращающее деда узла, если такой имеется
        /// иначе null
        /// </summary>
        public RBTreeNode<T> Grandparent => Parent?.Parent;

        #endregion

        #region Sibling
        
        /// <summary>
        /// Get-only свойство, возвращающее брата узла
        /// Если отец или соответсвующий брат отсутствуют, то null
        /// </summary>
        public RBTreeNode<T> Sibling => Parent == null 
            ? null
            : Parent.Left == this 
                ? Parent.Right 
                : Parent.Left;
        
        #endregion

        #region Uncle
        
        /// <summary>
        /// Get-only свойство, возвращающее дядю(брата отца) узла
        /// Если отец отсутствует, то null
        /// </summary>
        public RBTreeNode<T> Uncle => Parent?.Sibling;

        #endregion
        
        #region Color
        /// <summary>
        /// Авто-свойство для работы с цветом узла
        /// </summary>
        public NodeColor Color { get; internal set; }  

        #endregion

        #region Value
        
        /// <summary>
        /// Авто-свойство для работы с ключом узла
        /// </summary>
        public T Value { get; internal set; }
        
        #endregion
        
    }
}