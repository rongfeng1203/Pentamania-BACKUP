using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class RecipeBookManager : MonoBehaviour
{
    [SerializeField] private Material[] pageList;
    [SerializeField] private int[] maxPageFromTaskProgress; // determines the maximum page based on how many tasks have been completed.

    [SerializeField] private Vector3 returnPosition;
    [SerializeField] private float returnCheck = 0.9f; //defines at what y-level the book will return to position
    private float animSpeed = 10;

    private int taskProgress => TaskManager.Instance.taskFinished;
    private int currentPage;
    private Animator _animator;
    private int page1page;
    private int page2page;
    private int flippage;
    public Material page1mat;
    public Material page2mat;
    public Material flip1mat;
    public Material flip2mat;
    private bool flippability = true;

    void Start()
    {
        currentPage = 0;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        // check if book is inaccessible
        if (transform.position.y < returnCheck)
        {
            transform.position = returnPosition;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        page1mat = pageList[page1page * 2];
        page2mat = pageList[page2page * 2 + 1];
        flip1mat = pageList[flippage * 2 + 2];
        flip2mat = pageList[flippage * 2 + 1];

    }

    public void GoForward()
    {
        Debug.Log("Book: Go forward attempted");
        if (currentPage < maxPageFromTaskProgress[taskProgress] && flippability)
        {
            flippage = currentPage;
            currentPage++;
            flippability = false;
            _animator.SetTrigger("FlipLeft");
            StartCoroutine(WaitForLeftFlip());
            // TRIGGER ANIMATION

            // ADJUST THE PAGES
            Debug.Log("Book: Go forward done");
        }
    }

    public void GoBackward()
    {
        Debug.Log("Book: Go backward attempted");
        if (currentPage > 0 && flippability)
        {
            currentPage--;
            flippage = currentPage;
            flippability = false;
            _animator.SetTrigger("FlipRight");
            StartCoroutine(WaitForRightFlip());

            // TRIGGER ANIMATION

            // ADJUST THE PAGES
            Debug.Log("Book: Go backward done");
        }
    }

    IEnumerator WaitForLeftFlip()
    {
        yield return new WaitForSeconds(2.5f / animSpeed);
        page2page = currentPage;

        yield return new WaitForSeconds(1f / animSpeed);
        page1page = currentPage;
        page2page = currentPage;
        flippability = true;
        yield return null;
    }
    
    IEnumerator WaitForRightFlip()
    {
        yield return new WaitForSeconds(2.5f / animSpeed);
        page1page = currentPage;

        yield return new WaitForSeconds(1f / animSpeed);
        page1page = currentPage;
        page2page = currentPage;
        flippability = true;
        yield return null;
    }
    
    public void playPageFlip()
    {
        AudioManager.instance.PlaySound("book_page_flip", gameObject);
    }

    // dunno if you'll need this or not
    // just leaving it here in case
    // delete if you don't use it
    // public Material GetPageMaterial(int pageIndex)
    // {
    //     return pageList[pageIndex];
    // }
}
