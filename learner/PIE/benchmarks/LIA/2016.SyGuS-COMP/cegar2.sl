(set-logic LIA)

(synth-inv inv_fun ((x Int) (n Int) (m Int)))

(declare-primed-var x Int)
(declare-primed-var n Int)
(declare-primed-var m Int)

(define-fun pre_fun ((x Int) (n Int) (m Int)) Bool
(and (= x 0) (= m 0)))

(define-fun trans_fun ((x Int) (n Int) (m Int) (x! Int) (n! Int) (m! Int)) Bool
(or
(and (and (and (< x n) (= x! (+ x 1))) (= n! n)) (= m! m))

(and (and (and (< x n) (= x! (+ x 1))) (= n! n)) (= m! x))))


(define-fun post_fun ((x Int) (n Int) (m Int)) Bool
(not (and (and (>= x n) (> n 0))
(or (<= n m) (< m 0)))))

(inv-constraint inv_fun pre_fun trans_fun post_fun)

(check-synth)