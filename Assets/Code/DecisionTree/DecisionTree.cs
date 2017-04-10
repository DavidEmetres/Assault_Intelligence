using UnityEngine;
using System.Collections;

public class DecisionTree {

    private BaseNode root;
    private BaseNode currentNode;

    public DecisionTree(BaseNode root) {
        this.root = root;
        currentNode = root;
    }

    public void TraverseTree() {
        if(currentNode.Evaluate()) {
            currentNode = currentNode.GetLeftChild();
        }
        else {
            currentNode = currentNode.GetRightChild();
        }

        if (currentNode == null)
            currentNode = root;
    }
}
