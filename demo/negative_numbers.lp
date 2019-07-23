% negative_numbers.lp

% clingo allows negative numeric literals.
% (One would hope it does).

% > clingo negative_numbers.lp

smallest(X) :- X = #min{ -2, 0, 2}.

#show smallest/1.