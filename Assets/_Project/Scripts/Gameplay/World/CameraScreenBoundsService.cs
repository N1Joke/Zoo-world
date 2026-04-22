using UnityEngine;

namespace ZooWorld.Gameplay.World
{
    // Computes the play area rectangle from the camera viewport by projecting
    // its corners onto the ground plane (y = groundY).
    public class CameraScreenBoundsService : IScreenBoundsService
    {
        private readonly Camera _camera;
        private readonly float _groundY;
        private readonly float _padding;

        public CameraScreenBoundsService(Camera camera, float groundY, float padding = 0.5f)
        {
            _camera = camera;
            _groundY = groundY;
            _padding = padding;
        }

        public Vector3 Center
        {
            get
            {
                CalcRect(out Vector3 min, out Vector3 max);
                return new Vector3((min.x + max.x) * 0.5f, _groundY, (min.z + max.z) * 0.5f);
            }
        }

        public bool IsOutside(Vector3 worldPosition)
        {
            CalcRect(out Vector3 min, out Vector3 max);
            return worldPosition.x < min.x || worldPosition.x > max.x
                || worldPosition.z < min.z || worldPosition.z > max.z;
        }

        public Vector3 GetRandomPointInside()
        {
            CalcRect(out Vector3 min, out Vector3 max);
            float x = Random.Range(min.x + _padding, max.x - _padding);
            float z = Random.Range(min.z + _padding, max.z - _padding);
            return new Vector3(x, _groundY, z);
        }

        public Vector3 DirectionToCenter(Vector3 worldPosition)
        {
            Vector3 c = Center;
            Vector3 dir = new Vector3(c.x - worldPosition.x, 0f, c.z - worldPosition.z);
            return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.forward;
        }

        private void CalcRect(out Vector3 min, out Vector3 max)
        {
            if (_camera == null)
            {
                min = new Vector3(-10f, _groundY, -10f);
                max = new Vector3(10f, _groundY, 10f);
                return;
            }

            Vector3 bl = ViewportToGround(new Vector3(0f, 0f, 0f));
            Vector3 br = ViewportToGround(new Vector3(1f, 0f, 0f));
            Vector3 tl = ViewportToGround(new Vector3(0f, 1f, 0f));
            Vector3 tr = ViewportToGround(new Vector3(1f, 1f, 0f));

            float minX = Mathf.Min(Mathf.Min(bl.x, br.x), Mathf.Min(tl.x, tr.x));
            float maxX = Mathf.Max(Mathf.Max(bl.x, br.x), Mathf.Max(tl.x, tr.x));
            float minZ = Mathf.Min(Mathf.Min(bl.z, br.z), Mathf.Min(tl.z, tr.z));
            float maxZ = Mathf.Max(Mathf.Max(bl.z, br.z), Mathf.Max(tl.z, tr.z));

            min = new Vector3(minX, _groundY, minZ);
            max = new Vector3(maxX, _groundY, maxZ);
        }

        private Vector3 ViewportToGround(Vector3 viewport)
        {
            Ray ray = _camera.ViewportPointToRay(viewport);
            Plane ground = new Plane(Vector3.up, new Vector3(0f, _groundY, 0f));
            if (ground.Raycast(ray, out float t))
                return ray.GetPoint(t);

            Vector3 cam = _camera.transform.position;
            return new Vector3(cam.x, _groundY, cam.z);
        }
    }
}
