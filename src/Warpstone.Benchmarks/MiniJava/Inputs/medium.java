class Main {
    public static void main(String[] args) {
        BinarySearch search = new BinarySearch();
        BubbleSort sorter = new BubbleSort();
        LinkedList list = new LinkedList();
        BinaryTree tree = new BinaryTree();
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

class BinarySearch {
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

class BubbleSort {
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

class Node {
    int value;
    Node next;

    public int getValue() {
        return value;
    }

    public void setValue(int v) {
        value = v;
    }

    public Node getNext() {
        return next;
    }

    public void setNext(Node n) {
        next = n;
    }
}

class LinkedList {
    Node head;
    int size;

    public void init() {
        head = null;
        size = 0;
    }

    public void add(int value) {
        Node node = new Node();
        node.setValue(value);
        node.setNext(head);
        head = node;
        size = size + 1;
    }

    public int sum() {
        int total = 0;
        Node current = head;
        while (!(current == null)) {
            total = total + current.getValue();
            current = current.getNext();
        }
        return total;
    }

    public boolean contains(int value) {
        boolean found = false;
        Node current = head;
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

class TreeNode {
    int key;
    TreeNode left;
    TreeNode right;

    public void init(int k) {
        key = k;
        left = null;
        right = null;
    }
}

class BinaryTree {
    TreeNode root;
    int count;

    public void init(int key) {
        root = new TreeNode();
        root.init(key);
        count = 1;
    }

    public boolean insert(int key) {
        TreeNode current = root;
        boolean inserted = false;
        boolean done = false;
        while (!done) {
            if (key < current.key) {
                if (current.left == null) {
                    current.left = new TreeNode();
                    current.left.init(key);
                    inserted = true;
                    done = true;
                } else {
                    current = current.left;
                }
            } else if (key > current.key) {
                if (current.right == null) {
                    current.right = new TreeNode();
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
        TreeNode current = root;
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

    public int height(TreeNode node) {
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

class MathUtil extends Object {
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
