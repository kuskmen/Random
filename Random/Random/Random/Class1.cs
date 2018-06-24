using System;

namespace Random
{
    public class JobScheduler
    {
        public JobScheduler(Job job)
        {
            
        } 
    }

    public class Job
    {
        public Action Work { get; }

        public string Id { get; }

        public Job(Action work, string id)
        {
            Work = work;
            Id = id;
        }

    }
}
