using UnityEngine;
using System.Collections;

public interface BaseNode {

    bool Evaluate();
    void SetChildren(BaseNode leftChild, BaseNode rightChild);
    BaseNode GetLeftChild();
    BaseNode GetRightChild();
}
