using System.Collections;
using System.Collections.Generic;
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
