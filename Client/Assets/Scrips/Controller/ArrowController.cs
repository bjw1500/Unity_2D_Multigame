using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{

	float _heightArc = 1.0f;
	Vector3 _startPosition;
	Vector3 _targetPoint = Vector3.zero;
	float distance;


	public ObjectInfo _master;
	public float _speed;
	public SkillInfo skillinfo;
	public bool noDamage = false;

	

	//타겟 포인트에 곡선을 그리며 떨어지면 그만 아닌가?

	protected override void Init()
	{



		_startPosition = transform.position + new Vector3(0, 0.5f, 0);
		_speed = 10.0f;
		distance = (_startPosition - _target.transform.position).magnitude;
		_targetPoint = _target.transform.position;
		StartCoroutine("CoStartLiving");

	}

    protected override void UpdateController()
    {
		UpdateMoving();
    }

    protected override void UpdateAnimation()
	{

	}

    protected override void UpdateMoving()
    {

		if (_target == null)
			Managers.Resource.Destroy(gameObject);

		if (Mathf.Abs(_startPosition.x - _targetPoint.x) <=2 || distance <= 5.0f)
		{
			DownShot();
		}
		else
		{
			ParabolaShot();
		}

    }

	void ParabolaShot()
    {

		float x0 = _startPosition.x;
		float x1 = _targetPoint.x;
		//float distance = x1 - x0;
		float currentdistance = (transform.position - _startPosition).magnitude;

		float nextX = Mathf.MoveTowards(transform.position.x, x1, _speed * Time.deltaTime);
		//float nextX = Mathf.Lerp(transform.position.x, x1, _speed * Time.deltaTime);
		//float baseY = Mathf.Lerp(_StartPosition.y, _targetPoint.y, (nextX - x0) / distance);
		float baseY = Mathf.Lerp(_startPosition.y, _targetPoint.y, (currentdistance) / distance);
		float arc = _heightArc * (nextX - x0) * (nextX - x1) / (-0.10f * distance * distance);


		Vector3 nextPosition = new Vector3(nextX, baseY + arc, 0);

		transform.rotation = LookAt2D(nextPosition - transform.position);
		transform.position = nextPosition;


		if (distance - currentdistance < 0.2f)
			Arrived();


	}

	void DownShot()
    {


		Vector3 destPos = _targetPoint;
		Vector3 moveDir = destPos - transform.position;

		// 도착 여부 체크
		float dist = moveDir.magnitude;
		if (dist < _speed * Time.deltaTime)
		{
			transform.rotation = LookAt2D(destPos);
			transform.position = destPos;
			Arrived();
		}
		else
		{
			transform.rotation = LookAt2D(moveDir);
			transform.position += moveDir.normalized * _speed * Time.deltaTime;
		}
	}


	Quaternion LookAt2D(Vector2 forward)
	{
		return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
	}

	void Arrived()
	{
		//Debug.Log("도착");
		if(_target != null && noDamage == false)
        {
			CreatureController cc= _target.GetComponent<CreatureController>();
			cc.OnDamaged(this._master, skillinfo);
        }

		Managers.Resource.Destroy(gameObject);
	}

	IEnumerator CoStartLiving()
    {
		yield return new WaitForSeconds(5.0f);
		Managers.Resource.Destroy(gameObject);
	}


}

