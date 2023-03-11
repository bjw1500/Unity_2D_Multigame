using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GameObject
    {
        public GameRoom Room { get { return Player.Room; } set { Player.Room = value; }}
        public ObjectInfo Info { get; set; }
        public int UnitId { get { return Info.UnitId; } }

        public Player Player { get; set; }
        public StatInfo Stat { 
            get { return Info.StatInfo; }
            set { Info.StatInfo = value; } 
        } 

        public PositionInfo PosInfo
        {
            get { return Info.PosInfo; }
            set { Info.PosInfo = value; }
        }

        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            }

            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        public MoveDir Dir
        {
            get { return PosInfo.MoveDir; }
            set { PosInfo.MoveDir = value; }
        }

        public virtual CreatureState State
        {
            get { return Info.PosInfo.State; }
            set { PosInfo.State = value; }

        }



        public GameObject(ObjectInfo info, Player player, GameRoom room)
        {
            Info = info;
            Player = player;
            Room = room;
        }

        public virtual void Update()
        {


        }

        public void OnDamaged(GameObject Attacker, Skill skill)
        {

            //힐을 따로 만들까?
            if(skill.attackType == AttackType.Heal)
            {
                int totalDamage = Attacker.Stat.Damage + skill.damage;
                if (totalDamage < 0)
                    totalDamage = 0;

                Console.WriteLine($"{Attacker.Info.Name}가 {Info.Name}을 힐했습니다 {totalDamage}!");

                Stat.Hp = Math.Min(Stat.Hp + totalDamage, Stat.MaxHp);
                S_ChangeHp changePacket = new S_ChangeHp();
                changePacket.UnitId = UnitId;
                changePacket.Hp = Stat.Hp;
                Room.BroadCast(changePacket);
                return;
            }


            //데미지 판정
            {
                int totalDamage = Attacker.Stat.Damage + skill.damage - Info.StatInfo.Armor;
                if (totalDamage < 0)
                    totalDamage = 0;

                Console.WriteLine($"{Attacker.Info.Name}가 {Info.Name}을 공격했습니다 {totalDamage}!");

                Stat.Hp = Math.Max(Stat.Hp - totalDamage, 0);

                S_ChangeHp changePacket = new S_ChangeHp();
                changePacket.UnitId = UnitId;
                changePacket.Hp = Stat.Hp;
                Room.BroadCast(changePacket);

                //사망처리
                if (Stat.Hp <= 0)
                {
                    OnDead(Attacker);
                }
            }
        }
        
        public virtual void OnDead(GameObject Attacker)
        {

            Console.WriteLine($"{Info.Name}가 {Attacker.Info.Name}한테 죽었습니다.");

            GameObject OnDeadUnit = null;
         

            if (Player.PlayerUnits.TryGetValue(UnitId, out OnDeadUnit) == false)
                return;
            OnDeadUnit.Info.PosInfo.State = CreatureState.Dead;

            if(OnDeadUnit.Info.StatInfo.UnitType == UnitType.Building)
            {
                //성이 무너졌으니 패배.
                S_WinGame win = new S_WinGame();

                win.Win = 1;
                Attacker.Player.Session.Send(win);

                win.Win = 0;
                Player.Session.Send(win);


                return;
            }


            S_Die diePacket = new S_Die();
            diePacket.Attacker = Attacker.Info;
            diePacket.UnitId = UnitId;

            Room.BroadCast(diePacket);

            
        }

        public Vector2Int GetFrontCellPos()
        {
            Vector2Int cellPos = new Vector2Int(PosInfo.PosX,PosInfo.PosY);

            switch (PosInfo.MoveDir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
            }

            return cellPos;
        }

        public static MoveDir GetDirFromVec(Vector2Int dir)
        {
            if (dir.x > 0)
                return MoveDir.Right;
            else if (dir.x < 0)
                return MoveDir.Left;
            else if (dir.y > 0)
                return MoveDir.Up;
            else
                return MoveDir.Down;
        }

    }
}
