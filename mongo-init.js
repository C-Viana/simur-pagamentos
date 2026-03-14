rs.initiate({
  _id: "rs0",
  members: [
    { _id: 0, host: "mongo:27017" }
  ]
})

db = db.getSiblingDB("simurdb")
db.createCollection("payments")