using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Piece
    {
        public string PieceName { get; set; }
        public string Token { get; set; }
        public bool Alive { get; set; }
        public bool OwnedByWhite { get; set;}

        public int x { get; set; }
        public int y { get; set; }

        public Piece(bool whiteOwned)
        {
            this.OwnedByWhite = whiteOwned;
            this.Alive = true;
        }
    }

    class King : Piece
    {
        public King(bool whiteOwned)
            : base(whiteOwned)
        {
            Token = "K";
            PieceName = "King";
        }
    }

    class Queen : Piece
    { 
        public Queen(bool whiteOwned)
            : base(whiteOwned)
        {
            Token = "Q";
            PieceName = "Queen";
        }
    }

    class Rook : Piece
    { 
        public Rook(bool whiteOwned)
            : base(whiteOwned)
        {
            Token = "R";
            PieceName = "Rook";
        }
    }

    class Bishop : Piece
    { 
        public Bishop(bool whiteOwned)
            : base(whiteOwned)
        {
            Token = "B";
            PieceName = "Bishop";
        }
    }

    class Knight : Piece
    { 
        public Knight(bool whiteOwned)
            : base(whiteOwned)
        {
            Token = "N";
            PieceName = "Knight";
        }
    }

    class Pawn : Piece
    {
        public Pawn(bool whiteOwned)
            : base(whiteOwned)
        {
            Token = "P";
            PieceName = "Pawn";
        }
    }
}
