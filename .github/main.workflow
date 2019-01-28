workflow "Test" {
  on = "push"
  resolves = ["Run Cake task"]
}

action "Run Cake task" {
  uses = "gep13/cake-actions/task@master"
  args = "Test"
}
