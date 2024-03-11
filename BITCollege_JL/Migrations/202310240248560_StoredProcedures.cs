namespace BITCollege_JL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoredProcedures : DbMigration
    {
        public override void Up()
        {
            this.Sql(Properties.Resources.create_next_number);
        }
        
        public override void Down()
        {
            this.Sql(Properties.Resources.drop_next_number);
        }
    }
}
