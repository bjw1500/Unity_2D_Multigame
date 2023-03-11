using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Monster : GameObject
    {
		GameObject _target;
        public Monster(ObjectInfo info, Player player, GameRoom room) :base(info, player, room)
        {
            

        }

		public override CreatureState State
		{
			get { return Info.PosInfo.State; }
			set { 
				PosInfo.State = value;
				CheckUpdatePosition();
			}

		}


		public override void Update()
        {
          switch(State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Attack:
                    UpdateAttack();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;

            }
      
        }

		

		long _nextSearchTick = 0;
		protected virtual void UpdateIdle()
		{
			if (_nextSearchTick > Environment.TickCount64)
				return;
			_nextSearchTick = Environment.TickCount64 + 1000;

			GameObject target = Room.FindObject(p =>
			{
				if (Info.TeamId == p.Info.TeamId)
					return false;

				Vector2Int dir = p.CellPos - CellPos;
				return dir.cellDistFromZero <= Info.StatInfo.SearchRange;
			});

			if (target == null)
				return;

			_target = target;
			Info.TargetId = target.Info.UnitId;
			State = CreatureState.Moving;

		}

		long _nextMoveTick = 0;
		long _nextAttackTick = 0;
		protected virtual void UpdateMoving()
		{
			if (_nextMoveTick > Environment.TickCount64)
				return;
			int moveTick = (int)(1000 / Stat.Speed);
			_nextMoveTick = Environment.TickCount64 + moveTick;

			if (_target == null || _target.Room != Room)
			{
				_target = null;
				State = CreatureState.Idle;
				return;
			}

			//공격 전환.

			if(_target != null)
            {
				//사정거리 체크

				Vector2Int dir = _target.CellPos - CellPos;
				if (dir.magnitude <= Stat.SkillRange)
				{
					//if (_nextAttackTick2 > Environment.TickCount64)
					//	return;
					//int attackTick = (int)(1000 / Stat.AttackSpeed);
					//_nextAttackTick2 = Environment.TickCount64 + attackTick;

					//Dir = GetDirFromVec(dir);
					//State = CreatureState.Attack;
					//CheckUpdateAttack((int)Stat.AttackType);

					Dir = GetDirFromVec(dir);
					State = CreatureState.Attack;
					return;
				}
			}


			List<Vector2Int> path = Room._map.FindPath(CellPos, _target.CellPos, ignoreDestCollision: true);
			if (path.Count < 2 || path.Count > 40)
			{
				_target = null;
				State = CreatureState.Idle;
				return;
			}

			Vector2Int nextPos = path[1];
			Vector2Int moveCellDir = nextPos - CellPos;
			Dir = GetDirFromVec(moveCellDir);

			if (Room._map.CanGo(nextPos) && Room._map.Find(nextPos) == null)
			{
				Room._map.ApplyMove(this, path[1]);
			}
			else
			{
				State = CreatureState.Idle;
			}

	
			CheckUpdatePosition();

			// 다른 플레이어한테도 알려준다
		}


		long _coolTick = 0;
		protected virtual void UpdateAttack()
		{
			if (_coolTick == 0)
            {

				if(_target == null)
                {
					State = CreatureState.Idle;
					return;
                }

				Info.TargetId = _target.UnitId;

				//사정거리를 벗어났다면 다시 추적
				Vector2Int dir = _target.CellPos - CellPos;
				if (dir.magnitude > Stat.SkillRange)
				{
					State = CreatureState.Moving;
					return;
				}


				// 타게팅 방향 주시
				MoveDir lookDir = GetDirFromVec(dir);
				if (Dir != lookDir)
				{
					Dir = lookDir;
					CheckUpdatePosition();
				}


				//공격
				CheckUpdateAttack((int)Stat.AttackType);

				//데미지 판정
				Data.Skill SkillData = null;
				if (DataManager.SkillDict.TryGetValue((int)Stat.AttackType, out SkillData) == false)
					return;
				_target.OnDamaged(this, SkillData);
				_target = null;

				int coolTick = (int)(1000 / Stat.AttackSpeed);
				_coolTick = Environment.TickCount64 + coolTick;

            }

			//스킬 쿨다운
			if (_coolTick > Environment.TickCount64)
				return;
			_coolTick = 0;
		}

		protected virtual void UpdateDead()
		{

		}

		public void CheckUpdatePosition()
        {
            Console.WriteLine(
                Player.Info.PlayerId + " MovePacekt : " + PosInfo.PosX + "," + PosInfo.PosY);
            Console.WriteLine(Player.Info.PlayerId + " State:" + PosInfo.State.ToString());
            Console.WriteLine(Player.Info.PlayerId + " TeamId:" + Player.Info.TeamId);
			//Console.WriteLine("Target" + _target.Info.Name + "," + "위치:"+ _target.CellPos.x + " ,"+ _target.CellPos.y);


			S_Move movePacket = new S_Move();
			movePacket.PlayerId = Player.Info.PlayerId;
			movePacket.UnitId = UnitId;
			movePacket.PosInfo = PosInfo;
			Room.BroadCast(movePacket);
		}

		public void CheckUpdateAttack(int skillId)
        {
			S_Skill skill = new S_Skill();
			skill.Info = this.Info;
			skill.SkilId = skillId;
			Room.BroadCast(skill);

		}

        public override void OnDead(GameObject Attacker)
        {
            base.OnDead(Attacker);
			Room._map.ApplyLeave(this);
			Player.PlayerUnits.Remove(UnitId);

			S_Despawn S_Packet = new S_Despawn();
			S_Packet.Info.Add(this.Info);
			Room.Push(Room.BroadCast, S_Packet);

		}





    }
}
