using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    public abstract class Character
    {
        private static float    Range = 1.0f;
        private int             m_deathCount;
        private int             m_staminaMax;
        private int             m_staminaCurrent;
        private string          m_name;
        private float           m_respawnTime;
        private int             m_speed;
        private Team            m_team;
        private Flag            m_targetFlag;
        protected GameObject    m_pawn;
        protected GameObject    m_pawnGraphics;
        protected GameObject    m_graphics;
        protected GameObject    m_light;


        public string Name { get { return m_name; } set { m_name = value; } }
        public int DeathCount
        {
            get
            {
                return m_deathCount;
            }

            set
            {
                m_deathCount = value;
            }
        }
        public int StaminaMax
        {
            get
            {
                return m_staminaMax;
            }

            set
            {
                m_staminaMax = value;
            }
        }
        public int StaminaCurrent
        {
            get
            {
                return m_staminaCurrent;
            }

            set
            {
                m_staminaCurrent = value;
            }
        }
        public float RespawnTime
        {
            get
            {
                return m_respawnTime;
            }

            set
            {
                m_respawnTime = value;
            }
        }
        public int Speed
        {
            get
            {
                return m_speed;
            }

            set
            {
                m_speed = value;
            }
        }
        public Team Team
        {
             get
            {
                return m_team;
            }

            private set
            {
                m_team = value;
            }
        }
        public Vector2 Position 
        {
            get {
                Vector2 tmp;
                tmp.x = m_pawn.transform.position.x;
                tmp.y = m_pawn.transform.position.z;
                return tmp;
            }
            set {
                Vector3 vec;
                vec.x = value.x;
                vec.z = value.y;
                vec.y = m_pawn.transform.position.y;
                m_pawn.transform.position=vec;
                if (m_targetFlag != null)
                    m_targetFlag.updatePosition();
            }
        }
        public float Angle
        {
            get {
                Vector3 outAxis;
                float outAngle;
                m_pawn.transform.localRotation.ToAngleAxis(out outAngle, out outAxis);
                if (outAxis.y>0)
                    return outAngle * 2 * Mathf.PI / 360;
                else
                    return outAngle * -2 * Mathf.PI / 360;
            }
            set { m_pawn.transform.localRotation = Quaternion.AngleAxis(value * 360 / (2 * Mathf.PI), Vector3.up); }
        }
        public Vector2 Direction
        {
            get 
            { 
                Vector2 tmp;
                tmp.x=Mathf.Sin(Angle);
                tmp.y=Mathf.Cos(Angle);
                return tmp;
            }
            set 
            {
                
                Angle =-1.0f* Vector2.SignedAngle(Vector2.up, value.normalized)*Mathf.PI/180.0f;
            }
        }
        public abstract float Visibility { get; }
        public float GrabRange { get {return Character.Range;} }
        public virtual int Damage { get { return 1; } }// virtual in order to powerup damage in a type-specific manner

        public Character(Team t)
        {
            m_name = "DefaultName";
            m_team = t;
            m_pawn = new GameObject();
            m_pawn.transform.parent = Team.GraphicCharacterRoot.transform;
            m_pawnGraphics = new GameObject
            {
                name = "Graphics"
            };
            m_pawnGraphics.transform.parent = m_pawn.transform;
            m_graphics = GameObject.CreatePrimitive(PrimitiveType.Plane);
            m_graphics.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            m_graphics.transform.localScale = new Vector3(0.2f, 1.0f, 0.2f);
            m_graphics.transform.localRotation = Quaternion.AngleAxis(180,new Vector3(0.0f, 1.0f, 0.0f));
            m_graphics.transform.parent = m_pawnGraphics.transform;
            m_graphics.GetComponent<Renderer>().material = m_team.Material;
            Position = t.SpawnPoint.Position;
            //adding the component that "step" the character at each frame
            Pawn component = m_pawn.AddComponent<Pawn>();
            component.setCharacter(this);
            //the event come when the character has been "wakeUp"
            m_pawn.SetActive(false);
            m_targetFlag = null;
        }
        public void CleanGraphical()
        {
            if (m_pawn)
                GameObject.Destroy(m_pawn);
        }
        public void WakeUp()
        {
            Position = m_team.SpawnPoint.Position;
            m_pawn.SetActive(true);
        }
        public void Show(bool visibility)
        {
            m_pawnGraphics.SetActive(visibility);
        }
        public bool IsAlive()
        {
            return StaminaCurrent > 0;
        }
        public void Spawn()
        {
            StaminaCurrent = StaminaMax;
            Show(true);
            Position = Team.SpawnPoint.Position;
            Team.Game.NotifyCharacterRespawn(this);
        }
        public void SufferInjuries(Character source,int injuryAmount = 1)
        {
            StaminaCurrent -= injuryAmount;
            Team.Game.NotifyDamageInflicted(source, this,injuryAmount);
            if (StaminaCurrent <= 0)
            {
                StaminaCurrent = 0; // not mandatory
                DeathCount++;
                Show(false);
                if (m_targetFlag != null)
                {
                    m_targetFlag.Drop();
                    m_targetFlag = null;
                    Team.NotifyEnemyFlagDropped();
                }
                float basert;
                float.TryParse(Team.Game.GameConfiguration.GetItemValue("BaseRespawnTime"),out basert);
                basert *= DeathCount;// we simply multiply the death count with the base respawn time to create a cumulative punishement for dying
                m_respawnTime = basert;
                Team.Game.NotifyKill(this, source,basert);
            }
        }
        public void ChangeName(string name)
        {
            m_name = name;
            if (m_pawn != null)
                m_pawn.name = m_name;

        }
        public void Step(float delta)
        {
            if (IsAlive())
            {
                
                Attack();
                if (m_targetFlag != null)
                    m_targetFlag.updatePosition();
                Move(delta);
            }
            else
            { // wait for respawn
                m_respawnTime -= delta;
                if (m_respawnTime <= 0)
                {
                    Spawn();
                }
            }
        }

        protected void MoveTo(Vector2 target, float unit)
        {
            if ((target-Position).magnitude>0.001f)//this is to avoid flicking effect when target and position are too closed each other
                m_pawn.transform.LookAt(new Vector3(target.x,m_pawn.transform.position.y ,target.y));
            MoveForward(unit);
        }
        protected void MoveForward(float unit)
        {
            float mapHalfWidth = m_team.Game.Map.Width *0.5f;
            float mapHalfHeight = m_team.Game.Map.Height *0.5f;
            
            Vector3 target3f= m_pawn.transform.TransformPoint(Vector3.forward * unit);
            Vector2 newTarget;
            newTarget.x = target3f.x;
            newTarget.y = target3f.z;


            if (Mathf.Abs(newTarget.x) > mapHalfWidth)
            {
                Vector2 dir = Direction;
                dir.x *= -1;
                Direction = dir;
            }
            if (Mathf.Abs(newTarget.y) > mapHalfHeight)
            {
                Vector2 dir = Direction;
                dir.y *= -1;
                Direction = dir;
            }
            m_pawn.transform.Translate(0.0f, 0.0f, unit);
           // Debug.Log(Position+" => "+unit+" => "+Angle*Mathf.PI/180.0f);
           
        }
        protected bool IsSeeingPotentialTarget(out Flag f)
        { 
            f=null;
            return Team.Game.IsSeeingAPotentialTarget(this, out f);
        }
        protected bool TakeTheTarget()
        {
            if (Team.State != Team.TeamState.REACH) // to grab the flag, we should be in the Reach State
                return false;
            if (Team.Target == null)
                return false;
            if (Team.Target.Carrier != null)
                return false;
            if ((Team.Target.Position - Position).sqrMagnitude > GrabRange) //The target is out of range
                return false;
            Team.Target.Take( this);
            Team.NotifyEnemyFlagTaken();
            m_targetFlag = Team.Target;
            return true;
        }
        protected bool IsOnOwnFlagSpawn()
        {
            return ((Position - Team.Flag.SpawnPosition).magnitude < GrabRange);
        }
        protected void CaptureTheFlag()
        {
            if (m_targetFlag == null) return ;
            if (!IsOnOwnFlagSpawn()) return ;
            Team.Game.NotifyFlagCaptured(m_targetFlag);
            m_targetFlag.Drop();
            m_targetFlag = null;
            Team.NotifyEnemyFlagCaptured();          
        }
        protected virtual void Attack()  // virtual to make type-specific behavior possible, not done currently
        {//default implementation: attack the nearest enemy
            ArrayList enemies;
            if (!Team.Game.GetNearestEnemies(this, out enemies))
                return;// failed to get the enemie list
            if (enemies.Count <= 0) // there is no ennmy to attack
                return;

            if (!(enemies[0] is CharacterDistance))// shouldn't be
                return;
            CharacterDistance enemyDist = (CharacterDistance)enemies[0];

            if (enemyDist.distance < GrabRange) // if the closest enemy is at range
            {
                int enemyDamage = enemyDist.character.Damage;
                int ourDamage = Damage;
                Team.Game.NotifyCombat(this, enemyDist.character);
                enemyDist.character.SufferInjuries(this,ourDamage);
                SufferInjuries(enemyDist.character,enemyDamage);
            }

        }
        protected abstract void Move(float delta);// should be abstract if we want
        protected abstract void SetRespawnTransform();// type-specific respawn behavior
    }
}