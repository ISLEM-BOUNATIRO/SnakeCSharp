using System;
using System.Collections.Generic;

namespace Snake 
{ 
    public class Position 
    { 
        public int Row { get; set; }
        public int Col { get; set; }
        public Position (int row, int col) 
        { 
            Row = row; 
            Col = col;
        }
        public Position Translate(Direction dir) 
        { 
            Position p= new(Row + dir.RowOffset, Col + dir.ColOffset);
            return p;
        }

        public override bool Equals(object? obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Position? left, Position? right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position? left, Position? right)
        {
            return !(left == right);
        }
    }
}