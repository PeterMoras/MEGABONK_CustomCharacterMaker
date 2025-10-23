using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JigglePhysics;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class PhysBoneBaker
{
    public static JArray PhysBonesToJson(Transform rootTransform)
    {
        var physBones = rootTransform.GetComponentsInChildren<MyPhysicsBone>();
        JArray jbones = new JArray();
        foreach (var bone in physBones)
        {
            var jbone = new JObject();

            jbone["name"] = bone.transform.name;
            jbone["damping"] = bone.damping;
            jbone["dampingCurve"] = CurveToJson(bone.dampingCurve);
            jbone["elasticity"] = bone.elasticity;
            jbone["elasticityCurve"] = CurveToJson(bone.elasticityCurve);
            jbone["stiffness"] = bone.stiffness;
            jbone["stiffnessCurve"] = CurveToJson(bone.stiffnessCurve);
            jbone["inert"] = bone.inert;
            jbone["inertCurve"] = CurveToJson(bone.inertCurve);
            var g = bone.gravity;
            jbone["gravity"] = $"{g.x} {g.y} {g.z}";
            jbones.Add(jbone);
        }
        return jbones;
    }

    public static JArray JiggleBonesToJson(Transform rootTransform)
    {
        var rigs = rootTransform.GetComponentsInChildren<JiggleRigBuilder>();
        JArray jbones = new JArray();
        foreach (var rig in rigs)
        {
            
            var jbone = new JObject();
            jbone["rootName"] = rig.GetRootTransform().name;
            jbone["name"] = rig.gameObject.name;
            var data = rig.jiggleSettings as JiggleSettings;
            if (data == null)
            {
                Debug.LogWarning($"JiggleRig on {rootTransform.name} must use JiggleSettings.");
                continue;
            }
            jbone["data"] = JObject.FromObject(data.GetData());
            jbone["data"]["radiusCurve"] = CurveToJson(GetCurveFromJiggleSettings(data));
            var v = rig.wind;
            jbone["wind"] = $"{v.x} {v.y} {v.z}";
            jbone["colliders"] = JArray.FromObject(rig.colliders.Select(c => c.name));
            jbone["ignoredTransforms"] = JArray.FromObject(rig.ignoredTransforms.Select(t => t.name));
            
            jbones.Add(jbone);
        }

        return jbones;
    }

    static AnimationCurve GetCurveFromJiggleSettings(JiggleSettings jiggleSettings)
    {
        FieldInfo[] fields = typeof(JiggleSettings).GetFields(
            BindingFlags.NonPublic | 
            BindingFlags.Instance);
        var fieldCurve = fields.First(f => f.Name == "radiusCurve");
        return fieldCurve.GetValue(jiggleSettings) as AnimationCurve;
    }

    static JArray CurveToJson(AnimationCurve curve)
    {
        JArray jbones = new JArray();
        foreach (var key in curve.keys)
        {
            var boneKey = new JArray();
            boneKey.Add(key.time);
            boneKey.Add(key.value);
            jbones.Add(boneKey);
        }

        return jbones;
    }
}
