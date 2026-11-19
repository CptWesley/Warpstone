class Main1 {
    public static void main(String[] args) {
        BinarySearch1 search = new BinarySearch1();
        BubbleSort1 sorter = new BubbleSort1();
        LinkedList1 list = new LinkedList1();
        BinaryTree1 tree = new BinaryTree1();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch1 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort1 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node1 {
    int value;
    Node1 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node1 getNext() {
        return next;
    }

    public void setNext(Node1 n) {
        next = n;
    }
}

class LinkedList1 {
    Node1 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node1 node = new Node1();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node1 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node1 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode1 {
    int key;
    TreeNode1 left;
    TreeNode1 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree1 {
    TreeNode1 root;
    int count;

    public void init(int key) {
        root = new TreeNode1();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode1 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode1();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode1();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode1 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode1 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil1 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main2 {
    public static void main(String[] args) {
        BinarySearch2 search = new BinarySearch2();
        BubbleSort2 sorter = new BubbleSort2();
        LinkedList2 list = new LinkedList2();
        BinaryTree2 tree = new BinaryTree2();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch2 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort2 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node2 {
    int value;
    Node2 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node2 getNext() {
        return next;
    }

    public void setNext(Node2 n) {
        next = n;
    }
}

class LinkedList2 {
    Node2 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node2 node = new Node2();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node2 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node2 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode2 {
    int key;
    TreeNode2 left;
    TreeNode2 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree2 {
    TreeNode2 root;
    int count;

    public void init(int key) {
        root = new TreeNode2();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode2 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode2();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode2();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode2 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode2 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil2 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main3 {
    public static void main(String[] args) {
        BinarySearch3 search = new BinarySearch3();
        BubbleSort3 sorter = new BubbleSort3();
        LinkedList3 list = new LinkedList3();
        BinaryTree3 tree = new BinaryTree3();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch3 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort3 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node3 {
    int value;
    Node3 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node3 getNext() {
        return next;
    }

    public void setNext(Node3 n) {
        next = n;
    }
}

class LinkedList3 {
    Node3 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node3 node = new Node3();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node3 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node3 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode3 {
    int key;
    TreeNode3 left;
    TreeNode3 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree3 {
    TreeNode3 root;
    int count;

    public void init(int key) {
        root = new TreeNode3();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode3 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode3();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode3();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode3 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode3 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil3 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main4 {
    public static void main(String[] args) {
        BinarySearch4 search = new BinarySearch4();
        BubbleSort4 sorter = new BubbleSort4();
        LinkedList4 list = new LinkedList4();
        BinaryTree4 tree = new BinaryTree4();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch4 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort4 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node4 {
    int value;
    Node4 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node4 getNext() {
        return next;
    }

    public void setNext(Node4 n) {
        next = n;
    }
}

class LinkedList4 {
    Node4 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node4 node = new Node4();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node4 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node4 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode4 {
    int key;
    TreeNode4 left;
    TreeNode4 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree4 {
    TreeNode4 root;
    int count;

    public void init(int key) {
        root = new TreeNode4();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode4 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode4();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode4();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode4 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode4 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil4 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main5 {
    public static void main(String[] args) {
        BinarySearch5 search = new BinarySearch5();
        BubbleSort5 sorter = new BubbleSort5();
        LinkedList5 list = new LinkedList5();
        BinaryTree5 tree = new BinaryTree5();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch5 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort5 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node5 {
    int value;
    Node5 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node5 getNext() {
        return next;
    }

    public void setNext(Node5 n) {
        next = n;
    }
}

class LinkedList5 {
    Node5 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node5 node = new Node5();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node5 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node5 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode5 {
    int key;
    TreeNode5 left;
    TreeNode5 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree5 {
    TreeNode5 root;
    int count;

    public void init(int key) {
        root = new TreeNode5();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode5 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode5();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode5();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode5 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode5 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil5 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main6 {
    public static void main(String[] args) {
        BinarySearch6 search = new BinarySearch6();
        BubbleSort6 sorter = new BubbleSort6();
        LinkedList6 list = new LinkedList6();
        BinaryTree6 tree = new BinaryTree6();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch6 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort6 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node6 {
    int value;
    Node6 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node6 getNext() {
        return next;
    }

    public void setNext(Node6 n) {
        next = n;
    }
}

class LinkedList6 {
    Node6 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node6 node = new Node6();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node6 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node6 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode6 {
    int key;
    TreeNode6 left;
    TreeNode6 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree6 {
    TreeNode6 root;
    int count;

    public void init(int key) {
        root = new TreeNode6();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode6 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode6();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode6();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode6 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode6 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil6 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main7 {
    public static void main(String[] args) {
        BinarySearch7 search = new BinarySearch7();
        BubbleSort7 sorter = new BubbleSort7();
        LinkedList7 list = new LinkedList7();
        BinaryTree7 tree = new BinaryTree7();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch7 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort7 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node7 {
    int value;
    Node7 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node7 getNext() {
        return next;
    }

    public void setNext(Node7 n) {
        next = n;
    }
}

class LinkedList7 {
    Node7 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node7 node = new Node7();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node7 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node7 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode7 {
    int key;
    TreeNode7 left;
    TreeNode7 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree7 {
    TreeNode7 root;
    int count;

    public void init(int key) {
        root = new TreeNode7();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode7 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode7();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode7();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode7 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode7 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil7 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main8 {
    public static void main(String[] args) {
        BinarySearch8 search = new BinarySearch8();
        BubbleSort8 sorter = new BubbleSort8();
        LinkedList8 list = new LinkedList8();
        BinaryTree8 tree = new BinaryTree8();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch8 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort8 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node8 {
    int value;
    Node8 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node8 getNext() {
        return next;
    }

    public void setNext(Node8 n) {
        next = n;
    }
}

class LinkedList8 {
    Node8 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node8 node = new Node8();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node8 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node8 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode8 {
    int key;
    TreeNode8 left;
    TreeNode8 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree8 {
    TreeNode8 root;
    int count;

    public void init(int key) {
        root = new TreeNode8();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode8 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode8();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode8();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode8 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode8 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil8 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main9 {
    public static void main(String[] args) {
        BinarySearch9 search = new BinarySearch9();
        BubbleSort9 sorter = new BubbleSort9();
        LinkedList9 list = new LinkedList9();
        BinaryTree9 tree = new BinaryTree9();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch9 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort9 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node9 {
    int value;
    Node9 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node9 getNext() {
        return next;
    }

    public void setNext(Node9 n) {
        next = n;
    }
}

class LinkedList9 {
    Node9 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node9 node = new Node9();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node9 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node9 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode9 {
    int key;
    TreeNode9 left;
    TreeNode9 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree9 {
    TreeNode9 root;
    int count;

    public void init(int key) {
        root = new TreeNode9();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode9 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode9();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode9();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode9 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode9 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil9 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}

class Main10 {
    public static void main(String[] args) {
        BinarySearch10 search = new BinarySearch10();
        BubbleSort10 sorter = new BubbleSort10();
        LinkedList10 list = new LinkedList10();
        BinaryTree10 tree = new BinaryTree10();
        int[] numbers = new int[20];
        int i = 0;
        while (i < numbers.length) {
            numbers[i] = (i * 7 + 3) % 20;
            i = i + 1;
        }
        sorter.sort(numbers);
        System.out.println(search.find(numbers, 13));
        list.init();
        list.add(4);
        list.add(8);
        list.add(15);
        System.out.println(list.sum());
        tree.init(16);
        tree.insert(8);
        tree.insert(24);
        tree.insert(4);
        System.out.println(tree.contains(24) && !tree.contains(5));
        System.out.println("done");
    }
}

class BinarySearch10 {
    int comparisons;

    public int find(int[] values, int key) {
        int low = 0;
        int high = values.length - 1;
        int result = -1;
        boolean searching = true;
        comparisons = 0;
        while (searching && low <= high) {
            int mid = (low + high) / 2;
            comparisons = comparisons + 1;
            if (values[mid] == key) {
                result = mid;
                searching = false;
            } else if (values[mid] < key) {
                low = mid + 1;
            } else {
                high = mid - 1;
            }
        }
        return result;
    }

    public int getComparisons() {
        return comparisons;
    }
}

class BubbleSort10 {
    int swaps;

    public int sort(int[] values) {
        int n = values.length;
        int i = 0;
        swaps = 0;
        while (i < n - 1) {
            int j = 0;
            while (j < n - i - 1) {
                if (values[j] > values[j + 1]) {
                    int tmp = values[j];
                    values[j] = values[j + 1];
                    values[j + 1] = tmp;
                    swaps = swaps + 1;
                }
                j = j + 1;
            }
            i = i + 1;
        }
        return swaps;
    }

    public boolean isSorted(int[] values) {
        boolean sorted = true;
        int i = 1;
        while (sorted && i < values.length) {
            if (values[i - 1] > values[i]) {
                sorted = false;
            }
            i = i + 1;
        }
        return sorted;
    }
}

class Node10 {
    int value;
    Node10 next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node10 getNext() {
        return next;
    }

    public void setNext(Node10 n) {
        next = n;
    }
}

class LinkedList10 {
    Node10 head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node10 node = new Node10();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node10 current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node10 current = head;
        while (!found && current != null) {
            if (current.getValue() == value) {
                found = true;
            }
            current = current.getNext();
        }
        return found;
    }

    public int length() {
        return size;
    }
}

class TreeNode10 {
    int key;
    TreeNode10 left;
    TreeNode10 right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree10 {
    TreeNode10 root;
    int count;

    public void init(int key) {
        root = new TreeNode10();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode10 current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode10();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode10();
                    current.right.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.right;
                }
            } else {
                done = true;
            }
        }
        if (inserted) {
            count = count + 1;
        }
        return inserted;
    }

    public boolean contains(int key) {
        TreeNode10 current = root;
        boolean found = false;
        while (!found && current != null) {
            if (key == current.key) {
                found = true;
            } else if (key < current.key) {
                current = current.left;
            } else {
                current = current.right;
            }
        }
        return found;
    }

    public int height(TreeNode10 node) {
        int result = 0;
        if (node != null) {
            int l = this.height(node.left);
            int r = this.height(node.right);
            if (l > r) {
                result = l + 1;
            } else {
                result = r + 1;
            }
        }
        return result;
    }
}

class MathUtil10 extends Object {
    public static int gcd(int a, int b) {
        while (b != 0) {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static int power(int base, int exp) {
        int result = 1;
        while (exp > 0) {
            if (exp % 2 == 1) {
                result = result * base;
            }
            base = base * base;
            exp = exp / 2;
        }
        return result;
    }

    public static boolean isPrime(int n) {
        boolean prime = n >= 2;
        int d = 2;
        while (prime && d * d <= n) {
            if (n % d == 0) {
                prime = false;
            }
            d = d + 1;
        }
        return prime;
    }

    public static int fib(int n) {
        int a = 0;
        int b = 1;
        int i = 0;
        while (i < n) {
            int next = a + b;
            a = b;
            b = next;
            i = i + 1;
        }
        return a;
    }

    public static int clamp(int value, int min, int max) {
        int result = value;
        if (value < min || value > max) {
            if (value < min) {
                result = min;
            } else {
                result = max;
            }
        }
        return result;
    }
}
