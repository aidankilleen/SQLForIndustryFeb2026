SELECT COUNT(*) FROM Eircodes;

-- find duplicates

SELECT e.*
FROM Eircodes e
JOIN (
  SELECT Eircode, MIN(Id) AS KeepId
  FROM Eircodes
  GROUP BY Eircode
  HAVING COUNT(*) > 1
) d
ON d.Eircode = e.Eircode
WHERE e.Id <> d.KeepId
ORDER BY e.Eircode, e.Id;

-- copy duplicates to other table
DROP TABLE IF EXISTS EircodesDuplicates;

CREATE TABLE EircodesDuplicates AS
SELECT e.*
FROM Eircodes e
WHERE e.Eircode IN (
  SELECT Eircode
  FROM Eircodes
  GROUP BY Eircode
  HAVING COUNT(*) > 1
);

-- delete duplicates
DELETE FROM Eircodes
WHERE Eircode IN (
  SELECT Eircode
  FROM EircodesDuplicates
);