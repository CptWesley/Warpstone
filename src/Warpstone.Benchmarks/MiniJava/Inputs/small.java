class Factorial {
    public static void main(String[] args) {
        System.out.println(new Fac().compute(10));
    }
}

class Fac {
    int calls;

    public int compute(int num) {
        int result;
        calls = calls + 1;
        if (num < 1) {
            result = 1;
        } else {
            result = num * this.compute(num - 1);
        }
        return result;
    }

    public int iterative(int num) {
        int result = 1;
        int i = 2;
        while (i <= num) {
            result = result * i;
            i = i + 1;
        }
        return result;
    }
}
