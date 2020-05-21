all
#Allow ? in headers - used in FAQ
rule 'MD026', :punctuation => '.,;:!'
#Allow ordered lists by 123
rule 'MD029', :style => 'ordered'
#Allow long lines in tables
rule 'MD013', :tables  => false
#Allow inline HTML - using <br> for multiple lines in tables
exclude_rule 'MD033'
exclude_rule 'MD023' #False positives
