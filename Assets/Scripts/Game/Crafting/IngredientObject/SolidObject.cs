

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SolidObject : PassableIngredientObject
{
    [SerializeField] protected ItemStack ingredient;

    protected void Start()
    {
        Init(ingredient);
    }

    public override IngredientStack GetIngredient()
    {
        return ingredient;
    }

    public override void SetIngredient(IngredientStack ingredient)
    {
        if (ingredient is ItemStack item)
        {
            // handle logic for the change in ingredient
            this.ingredient = item;
            // set material and mesh
            /*
            gameObject.transform.localScale = solidIngredient.Def.GetScale();
            gameObject.GetComponent<MeshRenderer>().material = solidIngredient.Def.GetMaterial();
            gameObject.GetComponent<MeshFilter>().mesh = solidIngredient.Def.GetMesh();
            gameObject.GetComponent<MeshCollider>().sharedMesh = solidIngredient.Def.GetMesh();
            */
            transform.localScale = item.Def.GetScale();
            if (item.Def.GetPrefab().GetComponent<MeshRenderer>() != null)
            {
                if (gameObject.GetComponent<MeshRenderer>() != null)
                    gameObject.GetComponent<MeshRenderer>().material = item.Def.GetMaterial();
                else
                {
                    gameObject.AddComponent<MeshRenderer>().material = item.Def.GetMaterial();
                }
            }


            if (item.Def.GetPrefab().GetComponent<MeshFilter>() != null)
            {
                if (gameObject.GetComponent<MeshFilter>() != null)
                    gameObject.GetComponent<MeshFilter>().mesh = item.Def.GetMesh();
                else
                    gameObject.AddComponent<MeshFilter>().mesh = item.Def.GetMesh();
            }


            if (item.Def.GetPrefab().GetComponent<MeshCollider>() != null)
            {
                if (gameObject.GetComponent<MeshCollider>())
                    gameObject.GetComponent<MeshCollider>().sharedMesh = item.Def.GetMesh();
                else
                    gameObject.AddComponent<MeshCollider>().sharedMesh = item.Def.GetMesh();
            }

            /*
            else
            {
                var sourceCollider = item.Def.GetPrefab().GetComponent<Collider>();
                GetComponent<Collider>().enabled = false;
                System.Type colliderType = sourceCollider.GetType();
                Collider newCollider = this.gameObject.AddComponent(colliderType) as Collider;

                foreach (FieldInfo field in colliderType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (field.IsDefined(typeof(System.ObsoleteAttribute), true)) continue;
                    try
                    {
                        field.SetValue(newCollider, field.GetValue(sourceCollider));
                    }
                    catch { }
                }

                foreach (PropertyInfo prop in colliderType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!prop.CanWrite || !prop.CanRead || prop.GetIndexParameters().Length > 0)
                        continue;

                    if (prop.IsDefined(typeof(System.ObsoleteAttribute), true))
                        continue;

                    if (prop.Name == "material")
                    {
                        newCollider.sharedMaterial = sourceCollider.sharedMaterial;
                        continue;
                    }


                    try
                    {
                        prop.SetValue(newCollider, prop.GetValue(sourceCollider));
                    }
                    catch { }
                }
            }
            */

            /*
            foreach (Transform child in item.Def.GetPrefab().transform)
            {
                GameObject newChild = Instantiate(child.gameObject);

                newChild.transform.SetParent(transform);

                newChild.transform.localPosition = child.localPosition;
                newChild.transform.localRotation = child.localRotation;
                newChild.transform.localScale = child.localScale;
            }
            */
        }
        else
        {
            // throw some error
        }
    }

    public void Init(ItemStack item)
    {
        ingredient = item;
        transform.localScale = item.Def.GetScale();
        if (item.Def.GetPrefab().GetComponent<MeshRenderer>() != null)
        {
            if (gameObject.GetComponent<MeshRenderer>() != null)
                gameObject.GetComponent<MeshRenderer>().material = item.Def.GetMaterial();
            else
            {
                gameObject.AddComponent<MeshRenderer>().material = item.Def.GetMaterial();
            }
        }


        if (item.Def.GetPrefab().GetComponent<MeshFilter>() != null)
        {
            if (gameObject.GetComponent<MeshFilter>() != null)
                gameObject.GetComponent<MeshFilter>().mesh = item.Def.GetMesh();
            else
                gameObject.AddComponent<MeshFilter>().mesh = item.Def.GetMesh();
        }


        if (item.Def.GetPrefab().GetComponent<MeshCollider>() != null)
        {
            if (gameObject.GetComponent<MeshCollider>())
                gameObject.GetComponent<MeshCollider>().sharedMesh = item.Def.GetMesh();
            else
                gameObject.AddComponent<MeshCollider>().sharedMesh = item.Def.GetMesh();
        }
        /*
    else
    {
        var sourceCollider = item.Def.GetPrefab().GetComponent<Collider>();
        GetComponent<Collider>().enabled = false;
        System.Type colliderType = sourceCollider.GetType();
        Collider newCollider = this.gameObject.AddComponent(colliderType) as Collider;

        foreach (FieldInfo field in colliderType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.IsDefined(typeof(System.ObsoleteAttribute), true)) continue;
            try
            {
                field.SetValue(newCollider, field.GetValue(sourceCollider));
            }
            catch { }
        }

        foreach (PropertyInfo prop in colliderType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!prop.CanWrite || !prop.CanRead || prop.GetIndexParameters().Length > 0)
                continue;

            if (prop.IsDefined(typeof(System.ObsoleteAttribute), true))
                continue;

            if (prop.Name == "material")
            {
                newCollider.sharedMaterial = sourceCollider.sharedMaterial;
                continue;
            }


            try
            {
                prop.SetValue(newCollider, prop.GetValue(sourceCollider));
            }
            catch { }
        }
    }

    foreach (Transform child in item.Def.GetPrefab().transform)
    {
        GameObject newChild = Instantiate(child.gameObject);

        newChild.transform.SetParent(transform);

        newChild.transform.localPosition = child.localPosition;
        newChild.transform.localRotation = child.localRotation;
        newChild.transform.localScale = child.localScale;
    }
    */
    }

    public override void SetTags(List<IngredientTag> tags)
    {
        ingredient.tags = tags;
    }
}
