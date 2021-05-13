USE Fidelity

SELECT * 
FROM Card;

SELECT * 
FROM Operation;

SELECT * 
FROM Card INNER JOIN 
	 Operation ON Card.IDCard = Operation.IDCard;

SELECT *
FROM Card LEFT JOIN 
	 Operation ON Card.IDCard = Operation.IDCard;