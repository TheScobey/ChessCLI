using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Program
    {
        public static string PlayerName1;
        public static string PlayerName2;
        public static string PlayerTurnName;

        public static bool GameActive = true;
        public static bool WhitesTurn = true;
        public static bool TurnComplete = false;

        public static Cell[,] ChessBoard = new Cell[8,8];

        static void Main(string[] args)
        {

            PlayerName1 = "Scobey";
            PlayerName2 = "Computer";
            PlayerTurnName = PlayerName1;

            SetupBoard();

            while (GameActive)
            {
                if (TurnComplete)
                {
                    WhitesTurn = !WhitesTurn;
                    TurnComplete = false;

                    if (WhitesTurn)
                        PlayerTurnName = PlayerName1;
                    else
                        PlayerTurnName = PlayerName2;
                }
                DrawBoard();

                Console.WriteLine();
                Console.WriteLine("It is " + PlayerTurnName + "'s move.");

                string Input = Console.ReadLine().ToLower();

                if (Input != "" && Input.Length == 4 && Char.IsLetter(Input[0]) && Char.IsNumber(Input[1]) && Char.IsLetter(Input[2]) && Char.IsNumber(Input[3]))
                {
                    bool ValidCoord = true;

                    int OriginX = ConvertX(Input[0]);
                    int OriginY = ConvertY(Input[1]);
                    int TargetX = ConvertX(Input[2]);
                    int TargetY = ConvertY(Input[3]);

                    try
                    {
                        if (ChessBoard[OriginY, OriginX].PieceInCell.OwnedByWhite != WhitesTurn) // check if it is the right player trying to move the piece
                        {
                            Console.WriteLine("You cannot move a piece that isn't yours.");
                            ValidCoord = false;
                        }

                        string t = ChessBoard[TargetY, TargetX].Token; // try accessing the coordinate to see if it exists.
                    }
                    catch
                    {
                        Console.WriteLine("Invalid coordinate.");
                        ValidCoord = false;
                    }

                    if (ValidCoord)
                    {
                        Piece toMove = ChessBoard[
                            OriginY,
                            OriginX
                            ].PieceInCell;

                        if (CheckIfLegalMoveForPiece(TargetY, TargetX , toMove))
                        {

                            if (IsEmptySpace(TargetX, TargetY))
                            {
                                MovePiece(toMove, ConvertX(Input[2]), ConvertY(Input[3]));
                                TurnComplete = true;
                            }

                            if (IsEnemyPiece(TargetX, TargetY, toMove))
                            {
                                Console.WriteLine(GetPlayerName(toMove.OwnedByWhite) + "'s " + toMove.PieceName + " just took " + GetPlayerName(ChessBoard[TargetY, TargetX].PieceInCell.OwnedByWhite) +"'s " + ChessBoard[TargetY, TargetX].PieceInCell.PieceName);
                                Console.ReadKey();
                                ChessBoard[TargetY, TargetX].PieceInCell.Alive = false;
                                MovePiece(toMove, ConvertX(Input[2]), ConvertY(Input[3]));
                                TurnComplete = true;    
                            }
                        }
                        else
                        {
                            Console.WriteLine("Illegal move. Press any key to continue.");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                    }

                    if (Input == "quit" && CheckYes())
                    {
                        GameActive = false;
                    }

                    if (Input == "help")
                    {
                        Console.WriteLine("'Quit' to exit.");
                        Console.WriteLine("Moving format: '[cell][cell]' e.g. 'A2C5'");
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                    }
                }
            }

        }

        static string GetPlayerName(bool b)
        {
            if(b)
                return PlayerName1;

            return PlayerName2;
        }
        static bool CheckIfLegalMoveForPiece(int targetX, int targetY, Piece pieceToMove)
        {
            int MoveOffset = -1;

            if(pieceToMove.OwnedByWhite == true)
                MoveOffset = 1;

            switch (pieceToMove.PieceName)
            {
                case "King":
                    break;
                case "Queen":
                    if (targetX == pieceToMove.x || targetY == pieceToMove.y) //check if moving straight
                    {
                        if (DoesCollisionExistStraight(pieceToMove.x, pieceToMove.y, targetX, targetY, MoveOffset) == false)
                        {
                            return true;
                        }
                    }
                    break;
                case "Rook":
                    if (targetX == pieceToMove.x || targetY == pieceToMove.y) //check if moving straight
                    {
                        if (DoesCollisionExistStraight(pieceToMove.x, pieceToMove.y, targetX, targetY, MoveOffset) == false)
                        {
                            return true;
                        }
                    }
                    break;
                case "Bishop":
                    if (true) //check if moving diagonally
                    {
                        if (DoesCollisionExistDiagonal(pieceToMove.x, pieceToMove.y, targetX, targetY, MoveOffset) == false)
                        {
                            return true;
                        }
                    }
                    break;
                case "Knight":
                    break;
                case "Pawn":
                    if (ChessBoard[targetX, targetY].PieceInCell == null && targetX == pieceToMove.x + MoveOffset && targetY == pieceToMove.y) // normal 1 square move
                    {
                        return true;
                    }
                    else if (ChessBoard[targetX, targetY].PieceInCell == null && targetY == pieceToMove.y && targetX == pieceToMove.x + (MoveOffset * 2))
                    {
                        if (pieceToMove.OwnedByWhite == true && pieceToMove.x == 1)
                        {
                            return true;
                        }
                        else if (pieceToMove.OwnedByWhite == false && pieceToMove.x == 6)
                        {
                            return true;
                        }
                    }
                    else if (ChessBoard[targetX, targetY].PieceInCell != null && 
                        ChessBoard[targetX, targetY].PieceInCell.OwnedByWhite != pieceToMove.OwnedByWhite) // check if it is an enemy piece for diagonal taking
                        if (targetX == pieceToMove.x + MoveOffset && targetY == pieceToMove.y + 1 || targetY == pieceToMove.y - 1) // check it is actually diagonal
                            return true;
                    break;
                default:
                    Console.WriteLine("ERROR: PIECE DOES NOT EXIST. Press any key to continue.");
                    Console.ReadKey();
                    break;
            }

            return false;
        }

        static bool DoesCollisionExistDiagonal(int originX, int originY, int targetX, int targetY, int MoveOffset)
        {
            bool collisionFound = false;
            bool goingDown = targetX > originX;
            bool goingRight = targetY > originY;
            int xStep = -1;
            int yStep = -1;
            int targetSteps = 0;

            Console.WriteLine(goingDown + "," + goingRight);

            if (goingRight)
            {
                yStep = 1;
            }
            if(goingDown)
            {
                xStep = 1;
            }

            targetSteps = Math.Abs(originX - targetX) - 1;  // take 1 away from target steps so we don't check the square we're taking - 
                                                            //this will be checked higher up for enemy/friend

            for (int i = 0; i != targetSteps; i++)
            {
                Console.WriteLine("Checking for collision");
                Console.ReadLine();
                if (ChessBoard[i * xStep, i * yStep].PieceInCell != null)
                {
                    collisionFound = true;
                    Console.WriteLine("Collision found");
                }
            }


                //MoveOffset = Math.Sign(targetX - originX) * 1;
                //for (int i = originX + MoveOffset; i != targetX; i += MoveOffset)
                //{
                //    if (ChessBoard[i, targetY].PieceInCell != null)
                //    {
                //        collisionFound = true;
                //    }
                //}
                //MoveOffset = Math.Sign(targetY - originY) * 1;
                //for (int i = originY + MoveOffset; i != targetY; i += MoveOffset)
                //{
                //    if (ChessBoard[targetX, i].PieceInCell != null)
                //    {
                //        collisionFound = true;
                //    }

                Console.ReadLine();
            return collisionFound;
        }

        static bool DoesCollisionExistStraight(int originX, int originY, int targetX, int targetY, int MoveOffset)
        {
            bool collisionFound = false;

            if (originX != targetX)
            {
                MoveOffset = Math.Sign(targetX - originX) * 1;
                for (int i = originX + MoveOffset; i != targetX; i += MoveOffset)
                {
                    if (ChessBoard[i, targetY].PieceInCell != null)
                    {
                        collisionFound = true;
                    }
                }
            }
            else
            {
                MoveOffset = Math.Sign(targetY - originY) * 1;
                for (int i = originY + MoveOffset; i != targetY; i += MoveOffset)
                {
                    if (ChessBoard[targetX, i].PieceInCell != null)
                    {
                        collisionFound = true;
                    }
                }
            }

            return collisionFound;
        }

        static int GetDistance(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        static int ConvertX(int x)
        {
            return x - 97;
        }

        static int ConvertY(char y)
        {
            return 8 - (int)char.GetNumericValue(y);
        }

        static void MovePiece(Piece pieceToMove, int x, int y)
        {
            if (pieceToMove != null)
            {
                int originX = pieceToMove.x;
                int originY = pieceToMove.y;
 
                ChessBoard[y, x].PieceInCell = pieceToMove;
                ChessBoard[y, x].Token = pieceToMove.Token;
                pieceToMove.x = y;
                pieceToMove.y = x;
                ChessBoard[originX, originY].PieceInCell = null;
                ChessBoard[originX, originY].Token = ".";
            }
        }

        static bool IsEmptySpace(int x, int y)
        {
            if(ChessBoard[y,x].PieceInCell != null)
            {
                return false;
            }
            return true;
        }

        static bool IsEnemyPiece(int x, int y, Piece checkingPiece)
        {
            if (ChessBoard[y, x].PieceInCell != null && ChessBoard[y, x].PieceInCell.OwnedByWhite != checkingPiece.OwnedByWhite)
            {
                return true;
            }
            return false;

        }

        static bool CheckYes()
        {
            Console.WriteLine("Are you sure? y for yes, n for no.");
            if (Console.ReadLine().ToLower() == "y")
            {
                return true;
            }
            return false;
        }

        static void SetupBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    ChessBoard[x, y] = new Cell();
                    ChessBoard[x, y].Token = ".";
                }
            }

            SetupPlayer(true);
            SetupPlayer(false);
        }

        static void SetupPlayer(bool IsPlayerWhite)
        {
            int offsetY = 0;

            if (!IsPlayerWhite)
                offsetY = 7;

            SetPieceInCell(new Rook(IsPlayerWhite), offsetY, 0);
            SetPieceInCell(new Knight(IsPlayerWhite), offsetY, 1);
            SetPieceInCell(new Bishop(IsPlayerWhite), offsetY, 2);
            SetPieceInCell(new King(IsPlayerWhite), offsetY, 3);
            SetPieceInCell(new Queen(IsPlayerWhite), offsetY, 4);
            SetPieceInCell(new Bishop(IsPlayerWhite), offsetY, 5);
            SetPieceInCell(new Knight(IsPlayerWhite), offsetY, 6);
            SetPieceInCell(new Rook(IsPlayerWhite), offsetY, 7);

            if (!IsPlayerWhite)
                offsetY = 5;

            for (int i = 0; i < 8; i++)
            {
                SetPieceInCell(new Pawn(IsPlayerWhite), offsetY + 1, i);
            }
        }

        static void SetPieceInCell(Piece newPiece, int x, int y)
        {
            ChessBoard[x, y].PieceInCell = newPiece;
            ChessBoard[x, y].Token = newPiece.Token;
            newPiece.x = x;
            newPiece.y = y;
        }

        static void DrawBoard()
        {
            int count = 0;
            Console.Clear();
            Console.WriteLine(PlayerName1);
            Console.WriteLine("--BLACK---");
            Console.WriteLine("");
            Console.WriteLine("  abcdefgh");
            Console.Write(8 - count);
            Console.Write(" ");

            foreach (Cell c in ChessBoard)
            {
                if (count % 8 == 0 && count != 0)
                {
                    Console.Write(" ");
                    Console.Write(9 - (count / 8));
                    Console.WriteLine();
                    Console.Write(8 - (count / 8));
                    Console.Write(" ");
                }

                count++;
                Console.Write(c.Token);
            }
            Console.WriteLine(" 1");
            Console.WriteLine("  abcdefgh");
            Console.WriteLine();
            Console.WriteLine("--WHITE---");
            Console.WriteLine(PlayerName2);
        }
    }
}
